namespace QuickApp.Customer.Contracts
{
    /// <summary>Request payload for creating a new customer.</summary>
    public sealed record CreateCustomerRequest
    {
        public required string Name { get; init; }
        public required string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public Gender Gender { get; init; }
    }
}
