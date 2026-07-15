namespace QuickApp.Order.Contracts;

public interface ICustomerLookup
{
    Task<CustomerSnapshotDto?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default);
}
