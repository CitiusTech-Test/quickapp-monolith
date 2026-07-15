using QuickApp.Notification.Contracts;
using QuickApp.Notification.Contracts.Events;

namespace QuickApp.Notification.Service.Handlers
{
    // Stub handler: turns a UserRegistered event into a welcome email.
    // TODO: replace inline body with a real template renderer once wired to a broker.
    public class UserRegisteredHandler : INotificationHandler<UserRegistered>
    {
        private readonly IEmailSender _emailSender;

        public UserRegisteredHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public Task HandleAsync(UserRegistered @event, CancellationToken cancellationToken = default)
        {
            var recipientName = string.IsNullOrWhiteSpace(@event.FullName) ? @event.UserName : @event.FullName!;
            var subject = "Welcome to QuickApp";
            var body = $"Hi {recipientName}, thanks for registering with QuickApp.";

            return _emailSender.SendEmailAsync(
                recipientName,
                @event.Email,
                subject,
                body);
        }
    }
}
