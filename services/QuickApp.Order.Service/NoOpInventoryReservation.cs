using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

/// <summary>
/// Documented stub for <see cref="IInventoryReservation"/>. It performs NO real
/// reservation and always succeeds. Reservation semantics (hard/soft holds,
/// timeouts, oversell handling, compensation on failure) are intentionally out
/// of scope for the POC and must be designed during architecture review before a
/// production implementation replaces this type.
/// </summary>
public sealed class NoOpInventoryReservation : IInventoryReservation
{
    public Task<InventoryReservationResult> ReserveAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(InventoryReservationResult.Success());
    }
}
