using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

public sealed class InMemoryCustomerLookup(IEnumerable<CustomerSnapshotDto> customers)
    : ICustomerLookup
{
    private readonly IReadOnlyDictionary<int, CustomerSnapshotDto> _customers =
        customers.ToDictionary(customer => customer.CustomerId);

    public Task<CustomerSnapshotDto?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }
}
