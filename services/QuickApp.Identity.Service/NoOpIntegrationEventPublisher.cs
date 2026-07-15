using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Events;

namespace QuickApp.Identity.Service;

/// <summary>
/// Default no-op <see cref="IIntegrationEventPublisher"/> for the standalone skeleton.
/// A real deployment replaces this with a broker/outbox-backed publisher; downstream
/// consumers subscribe without Identity taking any dependency on them.
/// </summary>
public sealed class NoOpIntegrationEventPublisher : IIntegrationEventPublisher
{
    public Task PublishAsync(UserRegistered integrationEvent, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
