namespace QuickApp.Customer.Contracts
{
    /// <summary>Request payload for updating an existing customer.</summary>
    public class UpdateCustomerRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public Gender Gender { get; set; }
    }
}
