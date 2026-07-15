namespace QuickApp.Order.Contracts;

/// <summary>
/// Port for publishing Order integration events. Decouples the Order domain from
/// any concrete transport (in-process bus, message broker, outbox). Delivery
/// guarantees are an implementation concern and require manual architecture
/// review; the included stub records events in memory only.
/// </summary>
public interface IOrderEventPublisher
{
    Task PublishOrderPlacedAsync(
        OrderPlaced orderPlaced,
        CancellationToken cancellationToken = default);
}
