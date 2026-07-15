// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using QuickApp.Notification.Contracts;
using QuickApp.Notification.Contracts.Models;

namespace QuickApp.Notification.Service.Delivery
{
    /// <summary>
    /// A message captured by <see cref="InMemoryChannelSender"/>.
    /// </summary>
    public sealed record SentMessage(
        NotificationChannel Channel,
        string Address,
        string Subject,
        string Body);

    /// <summary>
    /// Deterministic in-memory stub sender used for the POC. It records every message
    /// it "sends" so tests and demos can assert on delivery, and it never contacts a
    /// real transport. A recipient with an empty address is reported as a failure so
    /// failure paths remain exercisable.
    /// </summary>
    public sealed class InMemoryChannelSender(ILogger<InMemoryChannelSender>? logger = null) : IChannelSender
    {
        private readonly ConcurrentQueue<SentMessage> _sent = new();

        public NotificationChannel Channel { get; init; } = NotificationChannel.Email;

        /// <summary>Messages recorded by this sender, in send order.</summary>
        public IReadOnlyCollection<SentMessage> SentMessages => _sent.ToArray();

        public Task<ChannelSendResult> SendAsync(
            NotificationRecipient recipient,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(recipient.Address))
            {
                logger?.LogWarning("Rejected notification for {Channel}: empty recipient address.", recipient.Channel);
                return Task.FromResult(new ChannelSendResult(false, "Recipient address is empty."));
            }

            _sent.Enqueue(new SentMessage(recipient.Channel, recipient.Address, subject, body));
            logger?.LogInformation("Stub-sent notification over {Channel} to {Address}.", recipient.Channel, recipient.Address);

            return Task.FromResult(new ChannelSendResult(true));
        }
    }
}
