// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

namespace QuickApp.Notification.Contracts.Models
{
    /// <summary>
    /// A channel-neutral description of who should receive a notification.
    /// The Notification domain does not know about Customer, Identity or any other
    /// service's entities; a recipient is described purely by an opaque reference
    /// and a per-channel address.
    /// </summary>
    /// <param name="Channel">The channel this address targets.</param>
    /// <param name="Address">The channel address (e.g. email address, phone number, device token, user reference).</param>
    /// <param name="DisplayName">Optional friendly name for rendering.</param>
    /// <param name="RecipientReference">Optional opaque reference supplied by the calling context (never a foreign key into another domain).</param>
    public sealed record NotificationRecipient(
        NotificationChannel Channel,
        string Address,
        string? DisplayName = null,
        string? RecipientReference = null);
}
