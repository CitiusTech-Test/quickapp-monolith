namespace QuickApp.Order.Contracts;

/// <summary>
/// Outcome of an <see cref="IInventoryReservation.ReserveAsync"/> attempt.
/// </summary>
public sealed record InventoryReservationResult
{
    public bool Succeeded { get; init; }
    public string? Reason { get; init; }

    public static InventoryReservationResult Success() => new() { Succeeded = true };

    public static InventoryReservationResult Failure(string reason) =>
        new() { Succeeded = false, Reason = reason };
}
