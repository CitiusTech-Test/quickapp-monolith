namespace QuickApp.Order.Contracts;

public sealed class OrderDetailDto
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
