namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Public contract for the standalone Customer service. Works exclusively with customer-owned
    /// DTOs; it deliberately exposes no order history, product, user-account or notification data.
    /// All operations are asynchronous and honour a <see cref="CancellationToken"/>.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>Returns a single customer by its integer id, or <c>null</c> when not found.</summary>
        Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists/searches customers ordered by name. An empty <see cref="CustomerSearchRequest.Query"/>
        /// returns all customers (subject to paging).
        /// </summary>
        Task<IReadOnlyList<CustomerDto>> SearchAsync(CustomerSearchRequest request, CancellationToken cancellationToken = default);

        /// <summary>Creates a customer and returns the new state plus a <see cref="CustomerChanged"/> event.</summary>
        Task<CustomerMutationResult> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing customer. Returns the updated state plus a <see cref="CustomerChanged"/>
        /// event, or <c>null</c> when no customer with <paramref name="id"/> exists.
        /// </summary>
        Task<CustomerMutationResult?> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    }
}
