// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

namespace QuickApp.Notification.Contracts.Models
{
    /// <summary>
    /// The delivery channels the Notification service is able to target.
    /// The skeleton is transport-agnostic; concrete senders decide how each channel is realized.
    /// </summary>
    public enum NotificationChannel
    {
        Email = 0,
        Sms = 1,
        Push = 2,
        InApp = 3
    }
}
