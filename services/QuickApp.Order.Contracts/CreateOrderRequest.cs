namespace QuickApp.Order.Contracts;

public sealed class CreateOrderRequest
{
    public int CustomerId { get; init; }
    public string? CashierUserId { get; init; }
    public decimal Discount { get; init; }
    public string? Comments { get; init; }
    public IReadOnlyCollection<CreateOrderDetailRequest> Items { get; init; } = [];
}
