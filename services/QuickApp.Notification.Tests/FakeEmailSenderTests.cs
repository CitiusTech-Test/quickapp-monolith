using QuickApp.Notification.Service;
using Xunit;

namespace QuickApp.Notification.Tests
{
    public class FakeEmailSenderTests
    {
        [Fact]
        public async Task SendEmailAsync_RecordsMessage()
        {
            var sender = new FakeEmailSender();

            var (success, error) = await sender.SendEmailAsync(
                "Alice", "alice@example.com", "Hello", "<p>Hi</p>");

            Assert.True(success);
            Assert.Null(error);

            var msg = Assert.Single(sender.SentMessages);
            Assert.Equal("Alice", msg.RecipientName);
            Assert.Equal("alice@example.com", msg.RecipientEmail);
            Assert.Equal("Hello", msg.Subject);
            Assert.True(msg.IsHtml);
            Assert.True(msg.Success);
        }

        [Fact]
        public async Task SendEmailAsync_WithSender_RecordsSenderFields()
        {
            var sender = new FakeEmailSender();

            await sender.SendEmailAsync(
                "QuickApp", "no-reply@quickapp.com",
                "Bob", "bob@example.com", "Subject", "Body", isHtml: false);

            var msg = Assert.Single(sender.SentMessages);
            Assert.Equal("QuickApp", msg.SenderName);
            Assert.Equal("no-reply@quickapp.com", msg.SenderEmail);
            Assert.Equal("bob@example.com", msg.RecipientEmail);
            Assert.False(msg.IsHtml);
        }
    }
}
