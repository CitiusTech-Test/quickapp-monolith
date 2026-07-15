namespace QuickApp.Customer.Contracts
{
    /// <summary>
    /// Result of a create/update operation: the current customer state plus the integration event
    /// describing the change. Returning the event (in addition to publishing it) lets callers
    /// forward it to Notification without Customer depending on any message-delivery component.
    /// </summary>
    public sealed record CustomerMutationResult
    {
        public required CustomerDto Customer { get; init; }

        public required CustomerChanged Event { get; init; }
    }
}
