using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

/// <summary>
/// Minimal composition entry point for the standalone Order service skeleton.
/// Registers the deterministic stub implementation and the documented stub ports.
/// A host replaces the port registrations with real adapters (Customer/Product
/// service clients, inventory reservation, event transport) without touching the
/// Order domain.
/// </summary>
public static class OrderServiceCollectionExtensions
{
    public static IServiceCollection AddOrderService(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IInventoryReservation, NoOpInventoryReservation>();
        services.TryAddSingleton<IOrderEventPublisher, InMemoryOrderEventPublisher>();
        services.TryAddSingleton<IOrdersService, InMemoryOrdersService>();
        return services;
    }
}
