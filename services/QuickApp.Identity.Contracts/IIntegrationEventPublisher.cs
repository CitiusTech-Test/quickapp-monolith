using QuickApp.Identity.Contracts.Events;

namespace QuickApp.Identity.Contracts;

/// <summary>
/// Outbound seam for publishing Identity integration events. Keeps downstream
/// consumers (e.g. Notification) out of the Identity service's dependency graph:
/// Identity depends only on this abstraction, and the transport/consumers are
/// wired at composition time. The skeleton ships with a no-op default.
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync(UserRegistered integrationEvent, CancellationToken cancellationToken = default);
}
