using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service
{
    /// <summary>
    /// Composition entry point for the standalone Customer service skeleton. A future host
    /// (minimal API, worker or the monolith during strangler-fig migration) calls
    /// <see cref="AddCustomerService"/> to wire up the service and its default dependencies.
    /// </summary>
    public static class CustomerServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomerService(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddSingleton<ICustomerEventPublisher, NullCustomerEventPublisher>();
            services.TryAddSingleton<ICustomerService, InMemoryCustomerService>();

            return services;
        }
    }
}
