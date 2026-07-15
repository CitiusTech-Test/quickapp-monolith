using QuickApp.Notification.Contracts;
using QuickApp.Notification.Contracts.Events;

namespace QuickApp.Notification.Service.Handlers
{
    // Stub handler: turns an OrderPlaced event into an order-confirmation email.
    // TODO: replace inline body with a real template renderer once wired to a broker.
    public class OrderPlacedHandler : INotificationHandler<OrderPlaced>
    {
        private readonly IEmailSender _emailSender;

        public OrderPlacedHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public Task HandleAsync(OrderPlaced @event, CancellationToken cancellationToken = default)
        {
            var subject = $"Order #{@event.OrderId} confirmed";
            var body = $"Hi {@event.CustomerName}, your order #{@event.OrderId} " +
                       $"for {@event.TotalAmount:C} has been placed.";

            return _emailSender.SendEmailAsync(
                @event.CustomerName,
                @event.CustomerEmail,
                subject,
                body);
        }
    }
}
