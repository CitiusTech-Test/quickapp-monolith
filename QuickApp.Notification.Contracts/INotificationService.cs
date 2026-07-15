// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using QuickApp.Notification.Contracts.Models;

namespace QuickApp.Notification.Contracts
{
    /// <summary>
    /// The primary entry point of the Notification domain. Callers submit delivery
    /// requests and query their status; the domain owns channel selection, template
    /// rendering and (optionally) durable delivery records.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Submit a notification request for delivery. Submissions carrying a repeated
        /// <see cref="NotificationRequest.IdempotencyKey"/> are deduplicated.
        /// </summary>
        Task<NotificationSubmissionResult> SubmitAsync(
            NotificationRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Query the current delivery status for a previously submitted request.
        /// Returns <c>null</c> when no delivery with the given id is known.
        /// </summary>
        Task<DeliveryStatusResult?> GetStatusAsync(
            string deliveryId,
            CancellationToken cancellationToken = default);
    }
}
