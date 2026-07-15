namespace QuickApp.Order.Contracts;

/// <summary>
/// Read model for a single order line. <see cref="ProductName"/> and
/// <see cref="UnitPrice"/> are purchase-time snapshots that never change if the
/// source product is later renamed or repriced.
/// </summary>
public sealed record OrderDetailDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public required string ProductName { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal Discount { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Total { get; init; }
}
