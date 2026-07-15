using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

public sealed class InMemoryOrdersService(
    ICustomerLookup customerLookup,
    IProductLookup productLookup,
    TimeProvider? timeProvider = null) : IOrdersService
{
    private readonly object _sync = new();
    private readonly List<Models.Order> _orders = [];
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;
    private int _nextOrderId = 1;
    private int _nextOrderDetailId = 1;

    public async Task<OrderDto> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);
        cancellationToken.ThrowIfCancellationRequested();

        var customer = await customerLookup.GetCustomerAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                $"Customer {request.CustomerId} was not found.");
        }

        var validatedItems = new List<ValidatedOrderItem>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var product = await productLookup.GetProductAsync(
                item.ProductId,
                cancellationToken);

            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product {item.ProductId} was not found.");
            }

            if (product.UnitPrice < 0)
            {
                throw new InvalidOperationException(
                    $"Product {item.ProductId} has an invalid unit price.");
            }

            var subtotal = product.UnitPrice * item.Quantity;
            if (item.Discount > subtotal)
            {
                throw new ArgumentException(
                    $"Discount for product {item.ProductId} exceeds its line subtotal.",
                    nameof(request));
            }

            validatedItems.Add(new ValidatedOrderItem(item, product));
        }

        var amountAfterLineDiscounts = validatedItems.Sum(item =>
            (item.Product.UnitPrice * item.Request.Quantity) - item.Request.Discount);

        if (request.Discount > amountAfterLineDiscounts)
        {
            throw new ArgumentException(
                "Order discount exceeds the amount after line-item discounts.",
                nameof(request));
        }

        lock (_sync)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var order = new Models.Order
            {
                Id = _nextOrderId++,
                Discount = request.Discount,
                Comments = request.Comments,
                CashierUserId = request.CashierUserId,
                CustomerId = customer.CustomerId,
                CustomerName = customer.Name,
                CustomerEmail = customer.Email,
                CreatedBy = request.CashierUserId,
                UpdatedBy = request.CashierUserId,
                CreatedDate = now,
                UpdatedDate = now
            };

            foreach (var item in validatedItems)
            {
                order.OrderDetails.Add(new Models.OrderDetail
                {
                    Id = _nextOrderDetailId++,
                    OrderId = order.Id,
                    ProductId = item.Product.ProductId,
                    ProductName = item.Product.Name,
                    UnitPrice = item.Product.UnitPrice,
                    Quantity = item.Request.Quantity,
                    Discount = item.Request.Discount,
                    CreatedBy = request.CashierUserId,
                    UpdatedBy = request.CashierUserId,
                    CreatedDate = now,
                    UpdatedDate = now
                });
            }

            _orders.Add(order);
            return Map(order);
        }
    }

    public Task<OrderDto?> GetOrderByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            var order = _orders.SingleOrDefault(order => order.Id == orderId);
            return Task.FromResult(order is null ? null : Map(order));
        }
    }

    public Task<IReadOnlyList<OrderDto>> ListOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            IReadOnlyList<OrderDto> orders = _orders
                .OrderBy(order => order.Id)
                .Select(Map)
                .ToArray();

            return Task.FromResult(orders);
        }
    }

    public Task<IReadOnlyList<OrderDto>> ListOrdersByCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(customerId));
        }

        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            IReadOnlyList<OrderDto> orders = _orders
                .Where(order => order.CustomerId == customerId)
                .OrderBy(order => order.Id)
                .Select(Map)
                .ToArray();

            return Task.FromResult(orders);
        }
    }

    private static void ValidateRequest(CreateOrderRequest request)
    {
        if (request.CustomerId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "Customer id must be positive.");
        }

        if (request.Discount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "Order discount cannot be negative.");
        }

        if (request.Items.Count == 0)
        {
            throw new ArgumentException(
                "An order must contain at least one line item.",
                nameof(request));
        }

        foreach (var item in request.Items)
        {
            if (item.ProductId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request),
                    "Product ids must be positive.");
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request),
                    "Line-item quantities must be positive.");
            }

            if (item.Discount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request),
                    "Line-item discounts cannot be negative.");
            }
        }
    }

    private static OrderDto Map(Models.Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            Discount = order.Discount,
            Comments = order.Comments,
            CashierUserId = order.CashierUserId,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Subtotal = order.Subtotal,
            LineItemDiscountTotal = order.LineItemDiscountTotal,
            Total = order.Total,
            CreatedDate = order.CreatedDate,
            CreatedBy = order.CreatedBy,
            Items = order.OrderDetails
                .OrderBy(item => item.Id)
                .Select(item => new OrderDetailDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Discount = item.Discount,
                    Subtotal = item.Subtotal,
                    Total = item.Total
                })
                .ToArray()
        };
    }

    private sealed record ValidatedOrderItem(
        CreateOrderDetailRequest Request,
        ProductSnapshotDto Product);
}
