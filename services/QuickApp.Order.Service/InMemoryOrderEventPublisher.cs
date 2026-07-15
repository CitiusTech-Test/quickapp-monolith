using System.Collections.Concurrent;
using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

/// <summary>
/// Documented stub for <see cref="IOrderEventPublisher"/>. It records published
/// events in memory only and provides no delivery guarantee, ordering, or
/// durability. A real transport (in-process bus, broker, or transactional
/// outbox) must replace it; delivery semantics require architecture review.
/// </summary>
public sealed class InMemoryOrderEventPublisher : IOrderEventPublisher
{
    private readonly ConcurrentQueue<OrderPlaced> _published = new();

    public IReadOnlyCollection<OrderPlaced> PublishedEvents => _published.ToArray();

    public Task PublishOrderPlacedAsync(
        OrderPlaced orderPlaced,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orderPlaced);
        cancellationToken.ThrowIfCancellationRequested();
        _published.Enqueue(orderPlaced);
        return Task.CompletedTask;
    }
}
