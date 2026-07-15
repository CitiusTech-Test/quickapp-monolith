// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

namespace QuickApp.Notification.Contracts.Models
{
    /// <summary>
    /// A request to deliver a notification. This is the Notification domain's own
    /// ingestion/request contract: other services translate their integration events
    /// into this shape rather than the Notification service depending on their event assemblies.
    /// </summary>
    public sealed record NotificationRequest
    {
        /// <summary>The recipients to deliver to. At least one is required.</summary>
        public required IReadOnlyList<NotificationRecipient> Recipients { get; init; }

        /// <summary>
        /// Logical template identifier used to render the message body/subject.
        /// The skeleton resolves this against a deterministic stub template catalog.
        /// </summary>
        public required string TemplateId { get; init; }

        /// <summary>Key/value data used to render the template.</summary>
        public IReadOnlyDictionary<string, string> Data { get; init; } =
            new Dictionary<string, string>();

        /// <summary>
        /// Optional idempotency key. Two requests carrying the same key are treated as
        /// the same logical submission, so retries do not create duplicate deliveries.
        /// </summary>
        public string? IdempotencyKey { get; init; }

        /// <summary>Optional correlation id propagated from the originating workflow for tracing.</summary>
        public string? CorrelationId { get; init; }

        /// <summary>
        /// Optional name of the source integration event type that triggered this request
        /// (a free-form string, not a reference to another service's event assembly).
        /// </summary>
        public string? SourceEventType { get; init; }
    }
}
