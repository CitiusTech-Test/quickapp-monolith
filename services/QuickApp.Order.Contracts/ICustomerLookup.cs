namespace QuickApp.Order.Contracts;

/// <summary>
/// Local port for validating customer existence and resolving the approved
/// customer snapshot. Belongs to the Order boundary and must be implemented by
/// an adapter (e.g. a Customer service client); it must not reference another
/// service's implementation.
/// </summary>
public interface ICustomerLookup
{
    Task<CustomerSnapshotDto?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default);
}
