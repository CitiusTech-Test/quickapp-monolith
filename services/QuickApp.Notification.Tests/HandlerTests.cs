using QuickApp.Notification.Contracts.Events;
using QuickApp.Notification.Service;
using QuickApp.Notification.Service.Handlers;
using Xunit;

namespace QuickApp.Notification.Tests
{
    public class HandlerTests
    {
        [Fact]
        public async Task OrderPlaced_TriggersConfirmationEmail()
        {
            var sender = new FakeEmailSender();
            var handler = new OrderPlacedHandler(sender);

            await handler.HandleAsync(new OrderPlaced
            {
                OrderId = 42,
                CustomerId = 7,
                CustomerName = "Alice",
                CustomerEmail = "alice@example.com",
                TotalAmount = 19.99m,
                PlacedAtUtc = DateTime.UtcNow
            });

            var msg = Assert.Single(sender.SentMessages);
            Assert.Equal("alice@example.com", msg.RecipientEmail);
            Assert.Contains("42", msg.Subject);
            Assert.Contains("Alice", msg.Body);
        }

        [Fact]
        public async Task UserRegistered_TriggersWelcomeEmail()
        {
            var sender = new FakeEmailSender();
            var handler = new UserRegisteredHandler(sender);

            await handler.HandleAsync(new UserRegistered
            {
                UserId = "u-1",
                UserName = "bob",
                Email = "bob@example.com",
                FullName = "Bob Jones",
                RegisteredAtUtc = DateTime.UtcNow
            });

            var msg = Assert.Single(sender.SentMessages);
            Assert.Equal("bob@example.com", msg.RecipientEmail);
            Assert.Contains("Welcome", msg.Subject);
            Assert.Contains("Bob Jones", msg.Body);
        }
    }
}
