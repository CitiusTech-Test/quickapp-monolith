namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Plain POCO representation of a customer exposed across the service boundary.
    /// Contains no EF/Identity dependencies and no navigation into other services (e.g. Orders).
    /// </summary>
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public Gender Gender { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
