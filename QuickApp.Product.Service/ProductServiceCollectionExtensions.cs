using Microsoft.Extensions.DependencyInjection;
using QuickApp.Product.Contracts;

namespace QuickApp.Product.Service;

public static class ProductServiceCollectionExtensions
{
    public static IServiceCollection AddProductService(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IProductService, StubProductService>();
        return services;
    }
}
