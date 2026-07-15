namespace QuickApp.Notification.Contracts.Events
{
    // Plain POCO event DTO. Carries only ids plus the snapshot data needed to render an
    // email. It deliberately does NOT reference Identity / ApplicationUser entities.
    public sealed record UserRegistered
    {
        public required string UserId { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public string? FullName { get; init; }
        public DateTime RegisteredAtUtc { get; init; }
    }
}
