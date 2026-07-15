using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service.Tests
{
    /// <summary>Test double that records every published <see cref="CustomerChanged"/> event.</summary>
    internal sealed class RecordingCustomerEventPublisher : ICustomerEventPublisher
    {
        public List<CustomerChanged> Published { get; } = new();

        public Task PublishAsync(CustomerChanged @event, CancellationToken cancellationToken = default)
        {
            Published.Add(@event);
            return Task.CompletedTask;
        }
    }
}
