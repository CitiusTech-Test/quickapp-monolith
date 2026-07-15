using QuickApp.Order.Contracts;
using QuickApp.Order.Service;

namespace QuickApp.Order.Service.Tests;

public sealed class InMemoryOrdersServiceTests
{
    private static readonly DateTimeOffset FixedUtcNow =
        new(2026, 7, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task PlaceOrderAsync_ComputesTotalsAndStoresLineSnapshots()
    {
        var publisher = new InMemoryOrderEventPublisher();
        var service = CreateService(publisher);

        var order = await service.PlaceOrderAsync(new CreateOrderRequest
        {
            CustomerId = 1,
            CashierUserId = "cashier-sub-123",
            Discount = 2m,
            Comments = "Multiple line items",
            Items =
            [
                new() { ProductId = 10, Quantity = 2, Discount = 1m },
                new() { ProductId = 20, Quantity = 3, Discount = 0.5m }
            ]
        });

        Assert.Equal(1, order.Id);
        Assert.Equal(OrderStatus.Placed, order.Status);
        Assert.Equal("cashier-sub-123", order.CashierUserId);
        Assert.Equal("Ada Lovelace", order.CustomerName);
        Assert.Equal("ada@example.com", order.CustomerEmail);
        Assert.Equal(35m, order.Subtotal);
        Assert.Equal(1.5m, order.LineItemDiscountTotal);
        Assert.Equal(31.5m, order.Total);
        Assert.Equal(FixedUtcNow.UtcDateTime, order.CreatedDate);
        Assert.Collection(
            order.Items,
            item =>
            {
                Assert.Equal(10, item.ProductId);
                Assert.Equal("Widget", item.ProductName);
                Assert.Equal(10m, item.UnitPrice);
                Assert.Equal(20m, item.Subtotal);
                Assert.Equal(19m, item.Total);
            },
            item =>
            {
                Assert.Equal(20, item.ProductId);
                Assert.Equal("Gadget", item.ProductName);
                Assert.Equal(5m, item.UnitPrice);
                Assert.Equal(15m, item.Subtotal);
                Assert.Equal(14.5m, item.Total);
            });
    }

    [Fact]
    public async Task PlaceOrderAsync_SnapshotsSurviveSourceDataChanges()
    {
        var productLookup = new MutableProductLookup(new ProductSnapshotDto
        {
            ProductId = 10,
            Name = "Widget",
            UnitPrice = 10m,
            IsAvailable = true
        });
        var service = new InMemoryOrdersService(
            new InMemoryCustomerLookup([Ada]),
            productLookup,
            new NoOpInventoryReservation(),
            new InMemoryOrderEventPublisher(),
            new FixedTimeProvider(FixedUtcNow));

        var placed = await service.PlaceOrderAsync(CreateRequest(customerId: 1));

        productLookup.Current = productLookup.Current with { Name = "Renamed", UnitPrice = 999m };

        var reloaded = await service.GetOrderByIdAsync(placed.Id);

        Assert.NotNull(reloaded);
        var line = reloaded.Items.Single();
        Assert.Equal("Widget", line.ProductName);
        Assert.Equal(10m, line.UnitPrice);
    }

    [Fact]
    public async Task PlaceOrderAsync_PublishesOrderPlacedEvent()
    {
        var publisher = new InMemoryOrderEventPublisher();
        var service = CreateService(publisher);

        var order = await service.PlaceOrderAsync(CreateRequest(customerId: 1));

        var evt = Assert.Single(publisher.PublishedEvents);
        Assert.Equal(order.Id, evt.OrderId);
        Assert.Equal(order.CustomerId, evt.CustomerId);
        Assert.Equal("ada@example.com", evt.CustomerEmail);
        Assert.Equal(order.Total, evt.Total);
        Assert.Equal(FixedUtcNow.UtcDateTime, evt.PlacedAtUtc);
        var line = Assert.Single(evt.Items);
        Assert.Equal(10, line.ProductId);
        Assert.Equal("Widget", line.ProductName);
    }

    [Fact]
    public async Task GetAndListMethods_ReturnStoredOrders()
    {
        var service = CreateService();
        var first = await service.PlaceOrderAsync(CreateRequest(customerId: 1));
        var second = await service.PlaceOrderAsync(CreateRequest(customerId: 2));

        var found = await service.GetOrderByIdAsync(second.Id);
        var all = await service.ListOrdersAsync();
        var missing = await service.GetOrderByIdAsync(999);

        Assert.NotNull(found);
        Assert.Equal(2, found.CustomerId);
        Assert.Null(missing);
        Assert.Equal([first.Id, second.Id], all.Select(order => order.Id));
    }

    [Fact]
    public async Task PlaceOrderAsync_UsesLookupsForValidation()
    {
        var customerLookup = new TrackingCustomerLookup(new CustomerSnapshotDto
        {
            CustomerId = 7,
            Name = "Grace Hopper",
            Email = "grace@example.com"
        });
        var productLookup = new TrackingProductLookup(new ProductSnapshotDto
        {
            ProductId = 42,
            Name = "Compiler",
            UnitPrice = 25m,
            IsAvailable = true
        });
        var service = new InMemoryOrdersService(
            customerLookup,
            productLookup,
            new NoOpInventoryReservation(),
            new InMemoryOrderEventPublisher());

        var order = await service.PlaceOrderAsync(new CreateOrderRequest
        {
            CustomerId = 7,
            Items = [new() { ProductId = 42, Quantity = 1 }]
        });

        Assert.Equal([7], customerLookup.RequestedIds);
        Assert.Equal([42], productLookup.RequestedIds);
        Assert.Equal("Grace Hopper", order.CustomerName);
        Assert.Equal("Compiler", order.Items.Single().ProductName);
    }

    [Fact]
    public async Task PlaceOrderAsync_RejectsUnknownCustomerOrProduct()
    {
        var missingCustomerService = CreateEmptyService();

        var customerError = await Assert.ThrowsAsync<InvalidOperationException>(
            () => missingCustomerService.PlaceOrderAsync(CreateRequest(customerId: 99)));

        var missingProductService = new InMemoryOrdersService(
            new InMemoryCustomerLookup([Ada]),
            new InMemoryProductLookup([]),
            new NoOpInventoryReservation(),
            new InMemoryOrderEventPublisher());

        var productError = await Assert.ThrowsAsync<InvalidOperationException>(
            () => missingProductService.PlaceOrderAsync(CreateRequest(customerId: 1)));

        Assert.Equal("Customer 99 was not found.", customerError.Message);
        Assert.Equal("Product 10 was not found.", productError.Message);
    }

    [Fact]
    public async Task PlaceOrderAsync_RejectsUnavailableProduct()
    {
        var service = new InMemoryOrdersService(
            new InMemoryCustomerLookup([Ada]),
            new InMemoryProductLookup(
            [
                new() { ProductId = 10, Name = "Widget", UnitPrice = 10m, IsAvailable = false }
            ]),
            new NoOpInventoryReservation(),
            new InMemoryOrderEventPublisher());

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.PlaceOrderAsync(CreateRequest(customerId: 1)));

        Assert.Equal("Product 10 is not available.", error.Message);
    }

    [Fact]
    public async Task PlaceOrderAsync_FailsAndDoesNotPublishWhenReservationFails()
    {
        var publisher = new InMemoryOrderEventPublisher();
        var service = new InMemoryOrdersService(
            new InMemoryCustomerLookup([Ada]),
            new InMemoryProductLookup(
            [
                new() { ProductId = 10, Name = "Widget", UnitPrice = 10m, IsAvailable = true }
            ]),
            new RejectingInventoryReservation(),
            publisher,
            new FixedTimeProvider(FixedUtcNow));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.PlaceOrderAsync(CreateRequest(customerId: 1)));

        Assert.Empty(publisher.PublishedEvents);
        Assert.Empty(await service.ListOrdersAsync());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task PlaceOrderAsync_RejectsNonPositiveCustomerId(int customerId)
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.PlaceOrderAsync(CreateRequest(customerId)));
    }

    [Fact]
    public async Task PlaceOrderAsync_RejectsEmptyOrder()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.PlaceOrderAsync(new CreateOrderRequest { CustomerId = 1, Items = [] }));
    }

    [Fact]
    public async Task PlaceOrderAsync_RejectsLineDiscountExceedingSubtotal()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.PlaceOrderAsync(new CreateOrderRequest
            {
                CustomerId = 1,
                Items = [new() { ProductId = 10, Quantity = 1, Discount = 100m }]
            }));
    }

    private static readonly CustomerSnapshotDto Ada = new()
    {
        CustomerId = 1,
        Name = "Ada Lovelace",
        Email = "ada@example.com"
    };

    private static InMemoryOrdersService CreateService(
        InMemoryOrderEventPublisher? publisher = null)
    {
        return new InMemoryOrdersService(
            new InMemoryCustomerLookup(
            [
                Ada,
                new() { CustomerId = 2, Name = "Alan Turing", Email = "alan@example.com" }
            ]),
            new InMemoryProductLookup(
            [
                new() { ProductId = 10, Name = "Widget", UnitPrice = 10m, IsAvailable = true },
                new() { ProductId = 20, Name = "Gadget", UnitPrice = 5m, IsAvailable = true }
            ]),
            new NoOpInventoryReservation(),
            publisher ?? new InMemoryOrderEventPublisher(),
            new FixedTimeProvider(FixedUtcNow));
    }

    private static InMemoryOrdersService CreateEmptyService()
    {
        return new InMemoryOrdersService(
            new InMemoryCustomerLookup([]),
            new InMemoryProductLookup([]),
            new NoOpInventoryReservation(),
            new InMemoryOrderEventPublisher());
    }

    private static CreateOrderRequest CreateRequest(int customerId)
    {
        return new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = [new() { ProductId = 10, Quantity = 1 }]
        };
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class MutableProductLookup(ProductSnapshotDto product) : IProductLookup
    {
        public ProductSnapshotDto Current { get; set; } = product;

        public Task<ProductSnapshotDto?> GetProductAsync(
            int productId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ProductSnapshotDto?>(
                Current.ProductId == productId ? Current : null);
        }
    }

    private sealed class RejectingInventoryReservation : IInventoryReservation
    {
        public Task<InventoryReservationResult> ReserveAsync(
            int productId,
            int quantity,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(InventoryReservationResult.Failure("out of stock"));
        }
    }

    private sealed class TrackingCustomerLookup(CustomerSnapshotDto customer) : ICustomerLookup
    {
        public List<int> RequestedIds { get; } = [];

        public Task<CustomerSnapshotDto?> GetCustomerAsync(
            int customerId,
            CancellationToken cancellationToken = default)
        {
            RequestedIds.Add(customerId);
            return Task.FromResult<CustomerSnapshotDto?>(
                customer.CustomerId == customerId ? customer : null);
        }
    }

    private sealed class TrackingProductLookup(ProductSnapshotDto product) : IProductLookup
    {
        public List<int> RequestedIds { get; } = [];

        public Task<ProductSnapshotDto?> GetProductAsync(
            int productId,
            CancellationToken cancellationToken = default)
        {
            RequestedIds.Add(productId);
            return Task.FromResult<ProductSnapshotDto?>(
                product.ProductId == productId ? product : null);
        }
    }
}
