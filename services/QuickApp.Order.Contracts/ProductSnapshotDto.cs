namespace QuickApp.Order.Contracts;

/// <summary>
/// Approved subset of product data the Order domain snapshots at place time,
/// including availability used for validation. Ownership of pricing and stock
/// remains with the Product domain.
/// </summary>
public sealed record ProductSnapshotDto
{
    public int ProductId { get; init; }
    public required string Name { get; init; }
    public decimal UnitPrice { get; init; }
    public bool IsAvailable { get; init; } = true;
    public int UnitsAvailable { get; init; }
}
