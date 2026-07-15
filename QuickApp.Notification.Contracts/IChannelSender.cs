// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using QuickApp.Notification.Contracts.Models;

namespace QuickApp.Notification.Contracts
{
    /// <summary>
    /// Result of handing a rendered message to a channel sender.
    /// </summary>
    /// <param name="Success">True when the sender accepted the message for delivery.</param>
    /// <param name="Detail">Optional detail, typically a failure reason.</param>
    public sealed record ChannelSendResult(bool Success, string? Detail = null);

    /// <summary>
    /// Transport-agnostic abstraction over a delivery channel. The POC ships a
    /// deterministic in-memory stub; a real deployment would adapt this to SMTP,
    /// an SMS gateway, a push provider, etc. without changing the Notification domain.
    /// </summary>
    public interface IChannelSender
    {
        /// <summary>The channel this sender handles.</summary>
        NotificationChannel Channel { get; }

        /// <summary>
        /// Deliver a rendered message to a recipient over this sender's channel.
        /// </summary>
        Task<ChannelSendResult> SendAsync(
            NotificationRecipient recipient,
            string subject,
            string body,
            CancellationToken cancellationToken = default);
    }
}
