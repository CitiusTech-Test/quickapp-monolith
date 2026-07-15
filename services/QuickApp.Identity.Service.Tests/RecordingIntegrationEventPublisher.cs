using QuickApp.Identity.Contracts;
using QuickApp.Identity.Contracts.Events;

namespace QuickApp.Identity.Service.Tests;

/// <summary>Test double that records published integration events for assertions.</summary>
internal sealed class RecordingIntegrationEventPublisher : IIntegrationEventPublisher
{
    public List<UserRegistered> Published { get; } = [];

    public Task PublishAsync(UserRegistered integrationEvent, CancellationToken cancellationToken = default)
    {
        Published.Add(integrationEvent);
        return Task.CompletedTask;
    }
}
