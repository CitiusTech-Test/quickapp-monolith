// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using QuickApp.Notification.Contracts;
using QuickApp.Notification.Contracts.Models;
using QuickApp.Notification.Service;
using QuickApp.Notification.Service.Delivery;
using QuickApp.Notification.Service.Templating;
using Xunit;

namespace QuickApp.Notification.Service.Tests
{
    public class StubNotificationServiceTests
    {
        private static (StubNotificationService service, InMemoryChannelSender sender) CreateService()
        {
            var sender = new InMemoryChannelSender { Channel = NotificationChannel.Email };
            var service = new StubNotificationService(new StubTemplateRenderer(), [sender]);
            return (service, sender);
        }

        private static NotificationRequest BuildRequest(
            string? idempotencyKey = null,
            string address = "user@example.com") => new()
            {
                Recipients = [new NotificationRecipient(NotificationChannel.Email, address, "User")],
                TemplateId = "welcome",
                Data = new Dictionary<string, string> { ["name"] = "Ada" },
                IdempotencyKey = idempotencyKey,
                CorrelationId = "corr-1",
                SourceEventType = "CustomerRegistered"
            };

        [Fact]
        public async Task SubmitAsync_DeliversAndReturnsDeliveredStatus()
        {
            var (service, sender) = CreateService();

            var result = await service.SubmitAsync(BuildRequest());

            Assert.False(result.Deduplicated);
            Assert.Equal(DeliveryStatus.Delivered, result.Status);
            Assert.NotEmpty(result.DeliveryId);
            Assert.Single(sender.SentMessages);
        }

        [Fact]
        public async Task GetStatusAsync_ReturnsStatusForSubmittedRequest()
        {
            var (service, _) = CreateService();

            var submission = await service.SubmitAsync(BuildRequest());
            var status = await service.GetStatusAsync(submission.DeliveryId);

            Assert.NotNull(status);
            Assert.Equal(submission.DeliveryId, status!.DeliveryId);
            Assert.Equal(DeliveryStatus.Delivered, status.Status);
            Assert.Equal(NotificationChannel.Email, status.Channel);
            Assert.Equal("corr-1", status.CorrelationId);
        }

        [Fact]
        public async Task GetStatusAsync_ReturnsNullForUnknownDelivery()
        {
            var (service, _) = CreateService();

            var status = await service.GetStatusAsync("does-not-exist");

            Assert.Null(status);
        }

        [Fact]
        public async Task SubmitAsync_WithRepeatedIdempotencyKey_IsDeduplicated()
        {
            var (service, sender) = CreateService();

            var first = await service.SubmitAsync(BuildRequest(idempotencyKey: "key-123"));
            var second = await service.SubmitAsync(BuildRequest(idempotencyKey: "key-123"));

            Assert.False(first.Deduplicated);
            Assert.True(second.Deduplicated);
            Assert.Equal(first.DeliveryId, second.DeliveryId);
            Assert.Single(sender.SentMessages);
        }

        [Fact]
        public async Task SubmitAsync_WithDistinctIdempotencyKeys_CreatesSeparateDeliveries()
        {
            var (service, sender) = CreateService();

            var first = await service.SubmitAsync(BuildRequest(idempotencyKey: "key-a"));
            var second = await service.SubmitAsync(BuildRequest(idempotencyKey: "key-b"));

            Assert.NotEqual(first.DeliveryId, second.DeliveryId);
            Assert.Equal(2, sender.SentMessages.Count);
        }

        [Fact]
        public async Task SubmitAsync_WithEmptyAddress_ReportsFailedDelivery()
        {
            var (service, _) = CreateService();

            var result = await service.SubmitAsync(BuildRequest(address: " "));
            var status = await service.GetStatusAsync(result.DeliveryId);

            Assert.Equal(DeliveryStatus.Failed, result.Status);
            Assert.NotNull(status);
            Assert.False(string.IsNullOrWhiteSpace(status!.Detail));
        }

        [Fact]
        public async Task SubmitAsync_WithUnroutableChannel_ReportsUndeliverable()
        {
            var service = new StubNotificationService(new StubTemplateRenderer(), []);

            var request = new NotificationRequest
            {
                Recipients = [new NotificationRecipient(NotificationChannel.Sms, "+15550000000")],
                TemplateId = "otp"
            };

            var result = await service.SubmitAsync(request);

            Assert.Equal(DeliveryStatus.Undeliverable, result.Status);
        }

        [Fact]
        public async Task SubmitAsync_WithNoRecipients_Throws()
        {
            var (service, _) = CreateService();

            var request = new NotificationRequest
            {
                Recipients = [],
                TemplateId = "welcome"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitAsync(request));
        }
    }
}
