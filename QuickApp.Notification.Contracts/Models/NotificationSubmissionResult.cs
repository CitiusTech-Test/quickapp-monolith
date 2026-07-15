// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

namespace QuickApp.Notification.Contracts.Models
{
    /// <summary>
    /// The outcome of submitting a <see cref="NotificationRequest"/>.
    /// </summary>
    /// <param name="DeliveryId">The service-assigned identifier used to query delivery status.</param>
    /// <param name="Status">The status of the request immediately after submission.</param>
    /// <param name="Deduplicated">
    /// True when an existing delivery with the same idempotency key was returned instead of
    /// creating a new one.
    /// </param>
    public sealed record NotificationSubmissionResult(
        string DeliveryId,
        DeliveryStatus Status,
        bool Deduplicated = false);
}
