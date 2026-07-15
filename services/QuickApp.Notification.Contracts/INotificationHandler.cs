namespace QuickApp.Notification.Contracts
{
    // In the target architecture the Notification service SUBSCRIBES to domain events
    // published by the Order and Identity services rather than being called in-process.
    // Each event type is handled by an INotificationHandler<T> implementation that renders
    // and dispatches the appropriate email.
    public interface INotificationHandler<in TEvent>
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}
