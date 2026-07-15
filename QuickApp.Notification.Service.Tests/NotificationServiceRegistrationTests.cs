// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using Microsoft.Extensions.DependencyInjection;
using QuickApp.Notification.Contracts;
using QuickApp.Notification.Contracts.Models;
using QuickApp.Notification.Service;
using Xunit;

namespace QuickApp.Notification.Service.Tests
{
    public class NotificationServiceRegistrationTests
    {
        [Fact]
        public async Task AddNotificationServiceStub_ResolvesAndDeliversEndToEnd()
        {
            var provider = new ServiceCollection()
                .AddNotificationServiceStub()
                .BuildServiceProvider();

            var service = provider.GetRequiredService<INotificationService>();

            var result = await service.SubmitAsync(new NotificationRequest
            {
                Recipients = [new NotificationRecipient(NotificationChannel.Email, "user@example.com")],
                TemplateId = "welcome"
            });

            Assert.Equal(DeliveryStatus.Delivered, result.Status);
        }
    }
}
