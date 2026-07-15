namespace QuickApp.Order.Contracts;

/// <summary>
/// Command to place an order. External domains are referenced only by scalar id;
/// the service resolves approved snapshot fields through its local ports.
/// </summary>
public sealed record CreateOrderRequest
{
    public int CustomerId { get; init; }
    public string? CashierUserId { get; init; }
    public decimal Discount { get; init; }
    public string? Comments { get; init; }
    public IReadOnlyCollection<CreateOrderDetailRequest> Items { get; init; } = [];
}
