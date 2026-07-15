// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using QuickApp.Notification.Contracts.Models;

namespace QuickApp.Notification.Service.Templating
{
    /// <summary>
    /// A rendered message ready to hand to a channel sender.
    /// </summary>
    public sealed record RenderedMessage(string Subject, string Body);

    /// <summary>
    /// Resolves a template id and request data into a concrete subject/body.
    /// Templates are owned by the Notification domain.
    /// </summary>
    public interface ITemplateRenderer
    {
        RenderedMessage Render(NotificationRequest request);
    }
}
