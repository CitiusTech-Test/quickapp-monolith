using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuickApp.Identity.Contracts;

namespace QuickApp.Identity.Service;

/// <summary>
/// Composition entry point for the standalone Identity service skeleton. Registers
/// the stub implementations and a default no-op event publisher. A host replaces the
/// <see cref="IIntegrationEventPublisher"/> registration with a real transport.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServiceStub(this IServiceCollection services)
    {
        services.TryAddSingleton<InMemoryIdentityStore>();
        services.TryAddSingleton<IIntegrationEventPublisher, NoOpIntegrationEventPublisher>();

        services.AddScoped<IUserDirectory, UserDirectory>();
        services.AddScoped<IRoleDirectory, RoleDirectory>();
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();

        return services;
    }
}
