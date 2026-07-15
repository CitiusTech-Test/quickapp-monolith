namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Integration event published when a customer is created, updated or deleted. Downstream
    /// domains (e.g. Notification) subscribe to this instead of Customer taking a direct dependency
    /// on an email sender. Carries only customer-owned data.
    /// </summary>
    public sealed record CustomerChanged
    {
        public required CustomerChangeKind ChangeKind { get; init; }

        public required int CustomerId { get; init; }

        public required string Name { get; init; }

        public required string Email { get; init; }

        public DateTime OccurredOnUtc { get; init; }
    }
}
