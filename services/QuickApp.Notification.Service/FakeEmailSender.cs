using System.Collections.Concurrent;
using QuickApp.Notification.Contracts;
using QuickApp.Notification.Service.Models;

namespace QuickApp.Notification.Service
{
    // Stub implementation of IEmailSender that records every sent message in memory instead of
    // talking to a real SMTP server (the monolith uses MailKit in QuickApp.Server EmailSender).
    // Deterministic and side-effect free so tests can assert on what was "sent".
    public class FakeEmailSender : IEmailSender
    {
        private readonly ConcurrentQueue<NotificationLog> _sent = new();

        public IReadOnlyList<NotificationLog> SentMessages => _sent.ToArray();

        public Task<(bool success, string? errorMsg)> SendEmailAsync(
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            return Record(null, null, recipientName, recipientEmail, subject, body, isHtml);
        }

        public Task<(bool success, string? errorMsg)> SendEmailAsync(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            return Record(senderName, senderEmail, recipientName, recipientEmail, subject, body, isHtml);
        }

        private Task<(bool success, string? errorMsg)> Record(
            string? senderName,
            string? senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml)
        {
            _sent.Enqueue(new NotificationLog
            {
                SenderName = senderName,
                SenderEmail = senderEmail,
                RecipientName = recipientName,
                RecipientEmail = recipientEmail,
                Subject = subject,
                Body = body,
                IsHtml = isHtml,
                Success = true,
                CreatedBy = "notification-service",
                CreatedDate = DateTime.UtcNow
            });

            return Task.FromResult<(bool success, string? errorMsg)>((true, null));
        }
    }
}
