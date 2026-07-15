using QuickApp.Order.Contracts;
using QuickApp.Order.Service;

namespace QuickApp.Order.Tests;

public sealed class InMemoryOrdersServiceTests
{
    private static readonly DateTimeOffset FixedUtcNow =
        new(2026, 7, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateOrderAsync_ComputesTotalsAndStoresSnapshots()
    {
        var service = CreateService();

        var order = await service.CreateOrderAsync(new CreateOrderRequest
        {
            CustomerId = 1,
            CashierUserId = "cashier-sub-123",
            Discount = 2m,
            Comments = "Multiple line items",
            Items =
            [
                new()
                {
                    ProductId = 10,
                    Quantity = 2,
                    Discount = 1m
                },
                new()
                {
                    ProductId = 20,
                    Quantity = 3,
                    Discount = 0.5m
                }
            ]
        });

        Assert.Equal(1, order.Id);
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
    public async Task GetAndListMethods_ReturnStoredOrdersAndFilterByCustomer()
    {
        var service = CreateService();
        var first = await service.CreateOrderAsync(CreateRequest(customerId: 1));
        var second = await service.CreateOrderAsync(CreateRequest(customerId: 2));
        var third = await service.CreateOrderAsync(CreateRequest(customerId: 1));

        var found = await service.GetOrderByIdAsync(second.Id);
        var all = await service.ListOrdersAsync();
        var customerOrders = await service.ListOrdersByCustomerAsync(1);

        Assert.NotNull(found);
        Assert.Equal(2, found.CustomerId);
        Assert.Equal([first.Id, second.Id, third.Id], all.Select(order => order.Id));
        Assert.Equal([first.Id, third.Id], customerOrders.Select(order => order.Id));
    }

    [Fact]
    public async Task CreateOrderAsync_UsesCustomerAndProductLookupsForValidation()
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
            UnitPrice = 25m
        });
        var service = new InMemoryOrdersService(customerLookup, productLookup);

        var order = await service.CreateOrderAsync(new CreateOrderRequest
        {
            CustomerId = 7,
            Items =
            [
                new()
                {
                    ProductId = 42,
                    Quantity = 1
                }
            ]
        });

        Assert.Equal([7], customerLookup.RequestedIds);
        Assert.Equal([42], productLookup.RequestedIds);
        Assert.Equal("Grace Hopper", order.CustomerName);
        Assert.Equal("Compiler", order.Items.Single().ProductName);
    }

    [Fact]
    public async Task CreateOrderAsync_RejectsUnknownCustomerOrProduct()
    {
        var missingCustomerService = new InMemoryOrdersService(
            new InMemoryCustomerLookup([]),
            new InMemoryProductLookup([]));

        var customerError = await Assert.ThrowsAsync<InvalidOperationException>(
            () => missingCustomerService.CreateOrderAsync(CreateRequest(customerId: 99)));

        var missingProductService = new InMemoryOrdersService(
            new InMemoryCustomerLookup(
            [
                new()
                {
                    CustomerId = 1,
                    Name = "Ada Lovelace",
                    Email = "ada@example.com"
                }
            ]),
            new InMemoryProductLookup([]));

        var productError = await Assert.ThrowsAsync<InvalidOperationException>(
            () => missingProductService.CreateOrderAsync(CreateRequest(customerId: 1)));

        Assert.Equal("Customer 99 was not found.", customerError.Message);
        Assert.Equal("Product 10 was not found.", productError.Message);
    }

    private static InMemoryOrdersService CreateService()
    {
        return new InMemoryOrdersService(
            new InMemoryCustomerLookup(
            [
                new()
                {
                    CustomerId = 1,
                    Name = "Ada Lovelace",
                    Email = "ada@example.com"
                },
                new()
                {
                    CustomerId = 2,
                    Name = "Alan Turing",
                    Email = "alan@example.com"
                }
            ]),
            new InMemoryProductLookup(
            [
                new()
                {
                    ProductId = 10,
                    Name = "Widget",
                    UnitPrice = 10m
                },
                new()
                {
                    ProductId = 20,
                    Name = "Gadget",
                    UnitPrice = 5m
                }
            ]),
            new FixedTimeProvider(FixedUtcNow));
    }

    private static CreateOrderRequest CreateRequest(int customerId)
    {
        return new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new()
                {
                    ProductId = 10,
                    Quantity = 1
                }
            ]
        };
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class TrackingCustomerLookup(CustomerSnapshotDto customer)
        : ICustomerLookup
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

    private sealed class TrackingProductLookup(ProductSnapshotDto product)
        : IProductLookup
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
