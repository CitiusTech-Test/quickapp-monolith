// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

namespace QuickApp.Notification.Contracts.Models
{
    /// <summary>
    /// The current delivery state of a previously submitted notification request.
    /// </summary>
    /// <param name="DeliveryId">The service-assigned delivery identifier.</param>
    /// <param name="Status">The current lifecycle status.</param>
    /// <param name="Channel">The channel the request was routed to, when known.</param>
    /// <param name="SubmittedAtUtc">When the request was submitted.</param>
    /// <param name="LastUpdatedAtUtc">When the status was last updated.</param>
    /// <param name="Detail">Optional human-readable detail (e.g. failure reason).</param>
    /// <param name="CorrelationId">Correlation id propagated from the originating request, when supplied.</param>
    public sealed record DeliveryStatusResult(
        string DeliveryId,
        DeliveryStatus Status,
        NotificationChannel? Channel,
        DateTimeOffset SubmittedAtUtc,
        DateTimeOffset LastUpdatedAtUtc,
        string? Detail = null,
        string? CorrelationId = null);
}
