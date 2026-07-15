using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service
{
    /// <summary>
    /// Domain model owned by the Customer service (table <c>AppCustomers</c> in the monolith).
    /// NOTE: the monolith's <c>ICollection&lt;Order&gt; Orders</c> navigation is intentionally
    /// REMOVED here — the Customer service must not know about the Order service's entities.
    /// </summary>
    public class Customer : BaseEntity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public Gender Gender { get; set; }
    }
}
