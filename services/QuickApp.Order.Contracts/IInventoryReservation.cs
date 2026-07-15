namespace QuickApp.Order.Contracts;

/// <summary>
/// Explicit abstraction for reserving stock while placing an order. Reservation
/// semantics (hard vs. soft holds, timeouts, compensation) are deliberately NOT
/// decided in this POC and require manual architecture review. Implementations
/// must document their behavior; the included stub performs no real reservation.
/// </summary>
public interface IInventoryReservation
{
    Task<InventoryReservationResult> ReserveAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default);
}
