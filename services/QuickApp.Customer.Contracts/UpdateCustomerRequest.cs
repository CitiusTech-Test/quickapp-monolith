namespace QuickApp.Customer.Contracts
{
    /// <summary>Request payload for updating an existing customer's profile, contact and address.</summary>
    public sealed record UpdateCustomerRequest
    {
        public required string Name { get; init; }
        public required string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public Gender Gender { get; init; }
    }
}
