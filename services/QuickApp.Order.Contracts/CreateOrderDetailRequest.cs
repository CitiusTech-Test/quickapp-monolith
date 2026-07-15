namespace QuickApp.Order.Contracts;

/// <summary>
/// Requested line item. Unit price and product name are not accepted from the
/// caller; they are snapshotted from <see cref="IProductLookup"/> at place time.
/// </summary>
public sealed record CreateOrderDetailRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Discount { get; init; }
}
