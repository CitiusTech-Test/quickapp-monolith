namespace QuickApp.Order.Contracts;

public sealed class CustomerSnapshotDto
{
    public int CustomerId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}
