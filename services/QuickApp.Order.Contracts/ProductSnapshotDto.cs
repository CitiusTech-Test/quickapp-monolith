namespace QuickApp.Order.Contracts;

public sealed class ProductSnapshotDto
{
    public int ProductId { get; init; }
    public required string Name { get; init; }
    public decimal UnitPrice { get; init; }
}
