using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service
{
    /// <summary>
    /// Default no-op <see cref="ICustomerEventPublisher"/>. Lets the Customer service run
    /// standalone in the POC; a real transport (e.g. a Notification integration) replaces it.
    /// </summary>
    public sealed class NullCustomerEventPublisher : ICustomerEventPublisher
    {
        public Task PublishAsync(CustomerChanged @event, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
