namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Public contract for the standalone Customer service. Mirrors the monolith's
    /// <c>QuickApp.Core.Services.Shop.ICustomerService</c> (<see cref="GetAllCustomersData"/>,
    /// <see cref="GetTopActiveCustomers"/>) but works exclusively with <see cref="CustomerDto"/>
    /// POCOs and adds standard CRUD.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>Returns all customers, ordered by name.</summary>
        IEnumerable<CustomerDto> GetAllCustomersData();

        /// <summary>
        /// Returns the top <paramref name="count"/> most active customers.
        /// Activity (order counts) is owned by the Order service and is NOT computed via a DB join
        /// here. Callers supply the ranking through <see cref="SetActivityRanking"/> (an id list the
        /// Order service provides); absent that, the stub falls back to a deterministic ordering.
        /// </summary>
        IEnumerable<CustomerDto> GetTopActiveCustomers(int count);

        CustomerDto? GetById(int id);
        CustomerDto Create(CreateCustomerRequest request);
        CustomerDto? Update(int id, UpdateCustomerRequest request);
        bool Delete(int id);

        /// <summary>
        /// Injects the customer-activity ranking (most-active first) as supplied by the Order
        /// service. This replaces the cross-service <c>Customer.Orders</c> navigation that existed
        /// in the monolith.
        /// </summary>
        void SetActivityRanking(IReadOnlyList<int> customerIdsByActivityDesc);
    }
}
