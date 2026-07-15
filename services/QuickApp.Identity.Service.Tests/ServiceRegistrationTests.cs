using Microsoft.Extensions.DependencyInjection;
using QuickApp.Identity.Contracts;

namespace QuickApp.Identity.Service.Tests;

public class ServiceRegistrationTests
{
    [Fact]
    public void AddIdentityServiceStub_resolves_all_contract_services()
    {
        var provider = new ServiceCollection()
            .AddIdentityServiceStub()
            .BuildServiceProvider();

        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUserDirectory>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IRoleDirectory>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUserRegistrationService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>());
    }

    [Fact]
    public void AddIdentityServiceStub_registers_noop_publisher_by_default()
    {
        var provider = new ServiceCollection()
            .AddIdentityServiceStub()
            .BuildServiceProvider();

        var publisher = provider.GetRequiredService<IIntegrationEventPublisher>();

        Assert.IsType<NoOpIntegrationEventPublisher>(publisher);
    }
}
