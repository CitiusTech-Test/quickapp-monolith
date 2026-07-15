namespace QuickApp.Order.Contracts;

/// <summary>
/// Approved subset of customer data the Order domain is allowed to snapshot.
/// The exact snapshot policy requires manual architecture review; this POC keeps
/// it explicit and minimal.
/// </summary>
public sealed record CustomerSnapshotDto
{
    public int CustomerId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}
