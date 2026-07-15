namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Abstraction for publishing <see cref="CustomerChanged"/> integration events. The Customer
    /// service depends on this rather than on any concrete message-delivery or email component;
    /// a Notification domain can supply an implementation later.
    /// </summary>
    public interface ICustomerEventPublisher
    {
        Task PublishAsync(CustomerChanged @event, CancellationToken cancellationToken = default);
    }
}
