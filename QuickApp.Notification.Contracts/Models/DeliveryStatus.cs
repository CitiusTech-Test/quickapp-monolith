// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

namespace QuickApp.Notification.Contracts.Models
{
    /// <summary>
    /// Lifecycle status of an outbound notification delivery request.
    /// </summary>
    public enum DeliveryStatus
    {
        /// <summary>The request was accepted and is awaiting processing.</summary>
        Pending = 0,

        /// <summary>The request is being handed to a channel sender.</summary>
        Sending = 1,

        /// <summary>The channel sender reported successful hand-off.</summary>
        Delivered = 2,

        /// <summary>Delivery failed; see <see cref="DeliveryStatusResult.Detail"/>.</summary>
        Failed = 3,

        /// <summary>The request could not be routed to any channel.</summary>
        Undeliverable = 4
    }
}
