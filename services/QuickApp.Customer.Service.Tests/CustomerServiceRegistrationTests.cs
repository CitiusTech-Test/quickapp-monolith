using Microsoft.Extensions.DependencyInjection;
using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service.Tests
{
    public class CustomerServiceRegistrationTests
    {
        [Fact]
        public void AddCustomerService_RegistersServiceAndDefaultPublisher()
        {
            var provider = new ServiceCollection()
                .AddCustomerService()
                .BuildServiceProvider();

            var service = provider.GetRequiredService<ICustomerService>();
            var publisher = provider.GetRequiredService<ICustomerEventPublisher>();

            Assert.IsType<InMemoryCustomerService>(service);
            Assert.IsType<NullCustomerEventPublisher>(publisher);
        }

        [Fact]
        public void AddCustomerService_DoesNotOverrideCustomPublisher()
        {
            var custom = new RecordingCustomerEventPublisher();

            var provider = new ServiceCollection()
                .AddSingleton<ICustomerEventPublisher>(custom)
                .AddCustomerService()
                .BuildServiceProvider();

            Assert.Same(custom, provider.GetRequiredService<ICustomerEventPublisher>());
        }

        [Fact]
        public async Task ResolvedService_CreatesCustomer_AndPublishesToRegisteredPublisher()
        {
            var recorder = new RecordingCustomerEventPublisher();
            var provider = new ServiceCollection()
                .AddSingleton<ICustomerEventPublisher>(recorder)
                .AddCustomerService()
                .BuildServiceProvider();

            var service = provider.GetRequiredService<ICustomerService>();
            var result = await service.CreateAsync(new CreateCustomerRequest { Name = "Dana", Email = "dana@example.com" });

            Assert.Equal("Dana", result.Customer.Name);
            Assert.Single(recorder.Published);
        }
    }
}
