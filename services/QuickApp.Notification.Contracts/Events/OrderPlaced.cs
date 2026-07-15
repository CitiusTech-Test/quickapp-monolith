namespace QuickApp.Notification.Contracts.Events
{
    // Plain POCO event DTO. Carries only ids plus the snapshot data needed to render an
    // email. It deliberately does NOT reference Order / Customer entities from other services.
    public sealed record OrderPlaced
    {
        public required int OrderId { get; init; }
        public required int CustomerId { get; init; }
        public required string CustomerName { get; init; }
        public required string CustomerEmail { get; init; }
        public decimal TotalAmount { get; init; }
        public DateTime PlacedAtUtc { get; init; }
    }
}
