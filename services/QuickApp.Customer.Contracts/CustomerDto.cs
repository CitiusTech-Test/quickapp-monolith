namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Customer-owned data exposed across the service boundary: profile, contact and address.
    /// Intentionally contains no order history, product, user-account or notification data and no
    /// navigation into other domains. Order history is provided by the Order service or a
    /// gateway/BFF read model, not by Customer.
    /// </summary>
    public sealed record CustomerDto
    {
        /// <summary>Integer identity, preserved from the monolith for the POC.</summary>
        public int Id { get; init; }

        public required string Name { get; init; }
        public required string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public Gender Gender { get; init; }

        public DateTime CreatedDate { get; init; }
        public DateTime UpdatedDate { get; init; }
    }
}
