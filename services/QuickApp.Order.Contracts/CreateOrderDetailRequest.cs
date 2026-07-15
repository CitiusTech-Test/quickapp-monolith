namespace QuickApp.Order.Contracts;

public sealed class CreateOrderDetailRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Discount { get; init; }
}
