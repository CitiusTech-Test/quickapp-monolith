// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using QuickApp.Notification.Contracts;
using QuickApp.Notification.Contracts.Models;
using QuickApp.Notification.Service.Templating;

namespace QuickApp.Notification.Service
{
    /// <summary>
    /// Deterministic, in-memory implementation of <see cref="INotificationService"/> for the
    /// standalone service skeleton. It owns channel selection, template rendering, idempotent
    /// submission and a durable-style (in-memory) delivery log. No real transport, broker,
    /// database or SMTP configuration is involved.
    /// </summary>
    public sealed class StubNotificationService : INotificationService
    {
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IReadOnlyDictionary<NotificationChannel, IChannelSender> _senders;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<StubNotificationService>? _logger;

        private readonly ConcurrentDictionary<string, DeliveryStatusResult> _deliveries = new();
        private readonly ConcurrentDictionary<string, string> _idempotencyIndex = new();

        public StubNotificationService(
            ITemplateRenderer templateRenderer,
            IEnumerable<IChannelSender> senders,
            TimeProvider? timeProvider = null,
            ILogger<StubNotificationService>? logger = null)
        {
            ArgumentNullException.ThrowIfNull(templateRenderer);
            ArgumentNullException.ThrowIfNull(senders);

            _templateRenderer = templateRenderer;
            _senders = senders.ToDictionary(s => s.Channel);
            _timeProvider = timeProvider ?? TimeProvider.System;
            _logger = logger;
        }

        public async Task<NotificationSubmissionResult> SubmitAsync(
            NotificationRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            if (request.Recipients.Count == 0)
                throw new ArgumentException("A notification request must have at least one recipient.", nameof(request));
            if (string.IsNullOrWhiteSpace(request.TemplateId))
                throw new ArgumentException("A notification request must specify a template id.", nameof(request));

            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey) &&
                _idempotencyIndex.TryGetValue(request.IdempotencyKey, out var existingId) &&
                _deliveries.TryGetValue(existingId, out var existing))
            {
                _logger?.LogInformation("Deduplicated submission for idempotency key {Key}.", request.IdempotencyKey);
                return new NotificationSubmissionResult(existing.DeliveryId, existing.Status, Deduplicated: true);
            }

            var deliveryId = Guid.NewGuid().ToString("N");
            var now = _timeProvider.GetUtcNow();
            var channel = request.Recipients[0].Channel;

            _deliveries[deliveryId] = new DeliveryStatusResult(
                deliveryId, DeliveryStatus.Pending, channel, now, now, CorrelationId: request.CorrelationId);

            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
                _idempotencyIndex[request.IdempotencyKey] = deliveryId;

            var result = await DispatchAsync(deliveryId, request, channel, cancellationToken).ConfigureAwait(false);
            return new NotificationSubmissionResult(deliveryId, result.Status);
        }

        public Task<DeliveryStatusResult?> GetStatusAsync(
            string deliveryId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _deliveries.TryGetValue(deliveryId, out var status);
            return Task.FromResult(status);
        }

        private async Task<DeliveryStatusResult> DispatchAsync(
            string deliveryId,
            NotificationRequest request,
            NotificationChannel channel,
            CancellationToken cancellationToken)
        {
            if (!_senders.TryGetValue(channel, out var sender))
                return Update(deliveryId, DeliveryStatus.Undeliverable, $"No sender registered for channel '{channel}'.");

            Update(deliveryId, DeliveryStatus.Sending, detail: null);

            var rendered = _templateRenderer.Render(request);
            var failures = new List<string>();

            foreach (var recipient in request.Recipients.Where(r => r.Channel == channel))
            {
                var send = await sender
                    .SendAsync(recipient, rendered.Subject, rendered.Body, cancellationToken)
                    .ConfigureAwait(false);

                if (!send.Success)
                    failures.Add(send.Detail ?? "Unknown delivery failure.");
            }

            return failures.Count == 0
                ? Update(deliveryId, DeliveryStatus.Delivered, detail: null)
                : Update(deliveryId, DeliveryStatus.Failed, string.Join("; ", failures));
        }

        private DeliveryStatusResult Update(string deliveryId, DeliveryStatus status, string? detail)
        {
            var updated = _deliveries.AddOrUpdate(
                deliveryId,
                _ => throw new InvalidOperationException($"Unknown delivery '{deliveryId}'."),
                (_, current) => current with
                {
                    Status = status,
                    Detail = detail ?? current.Detail,
                    LastUpdatedAtUtc = _timeProvider.GetUtcNow()
                });

            return updated;
        }
    }
}
