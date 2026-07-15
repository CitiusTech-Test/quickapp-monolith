using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

/// <summary>
/// Stub <see cref="ICustomerLookup"/> backed by a fixed set of snapshots. Replace
/// with a Customer service client adapter in a real deployment.
/// </summary>
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
