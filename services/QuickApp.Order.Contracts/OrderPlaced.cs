namespace QuickApp.Order.Contracts;

/// <summary>
/// Integration event emitted after an order is successfully placed. This is the
/// future seam for cross-domain reactions (e.g. Notification). Consumers must
/// not be called directly by the Order domain; they subscribe to this event.
/// </summary>
public sealed record OrderPlaced
{
    public required int OrderId { get; init; }
    public int CustomerId { get; init; }
    public required string CustomerEmail { get; init; }
    public string? CashierUserId { get; init; }
    public decimal Total { get; init; }
    public DateTime PlacedAtUtc { get; init; }
    public IReadOnlyList<OrderPlacedLineItem> Items { get; init; } = [];
}

/// <summary>
/// Line-item projection carried by <see cref="OrderPlaced"/>.
/// </summary>
public sealed record OrderPlacedLineItem
{
    public int ProductId { get; init; }
    public required string ProductName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
