// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickApp.Notification.Contracts;
using QuickApp.Notification.Service.Delivery;
using QuickApp.Notification.Service.Templating;

namespace QuickApp.Notification.Service
{
    /// <summary>
    /// Composition entry point for the standalone Notification service skeleton.
    /// A host (future standalone process or test harness) calls this to wire up the
    /// deterministic stub implementation. It intentionally registers no transport,
    /// broker or SMTP configuration.
    /// </summary>
    public static class NotificationServiceRegistration
    {
        public static IServiceCollection AddNotificationServiceStub(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddSingleton(TimeProvider.System);
            services.TryAddSingleton<ITemplateRenderer, StubTemplateRenderer>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IChannelSender, InMemoryChannelSender>());
            services.TryAddSingleton<INotificationService, StubNotificationService>();

            return services;
        }
    }
}
