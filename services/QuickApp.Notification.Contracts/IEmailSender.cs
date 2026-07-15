namespace QuickApp.Notification.Contracts
{
    // Extracted verbatim from QuickApp.Core/Services/IEmailSender.cs.
    // This is the outbound notification transport contract owned by the Notification service.
    public interface IEmailSender
    {
        Task<(bool success, string? errorMsg)> SendEmailAsync(
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true);

        Task<(bool success, string? errorMsg)> SendEmailAsync(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true);
    }
}
