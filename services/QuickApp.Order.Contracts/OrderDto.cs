namespace QuickApp.Order.Contracts;

/// <summary>
/// Read model for an order header plus its immutable purchase-time snapshots.
/// External domains are referenced only by scalar id and by approved snapshot
/// fields; no Customer/Product/ApplicationUser ORM types cross this boundary.
/// </summary>
public sealed record OrderDto
{
    public int Id { get; init; }
    public OrderStatus Status { get; init; }
    public decimal Discount { get; init; }
    public string? Comments { get; init; }

    public string? CashierUserId { get; init; }

    public int CustomerId { get; init; }
    public required string CustomerName { get; init; }
    public required string CustomerEmail { get; init; }

    public decimal Subtotal { get; init; }
    public decimal LineItemDiscountTotal { get; init; }
    public decimal Total { get; init; }

    public DateTime CreatedDate { get; init; }
    public string? CreatedBy { get; init; }

    public IReadOnlyList<OrderDetailDto> Items { get; init; } = [];
}
