using Microsoft.Extensions.DependencyInjection;
using QuickApp.Product.Contracts;
using QuickApp.Product.Service;
using Xunit;

namespace QuickApp.Product.Service.Tests;

public sealed class StubProductServiceTests
{
    private readonly IProductService _service = new StubProductService();

    [Fact]
    public async Task GetProductByIdReturnsOwnedCatalogData()
    {
        ProductDto? product = await _service.GetProductByIdAsync(2);

        Assert.NotNull(product);
        Assert.Equal("Widget Pro", product.Name);
        Assert.Equal(1, product.ProductCategoryId);
        Assert.Equal(1, product.ParentId);
        Assert.Equal(25m, product.SellingPrice);
    }

    [Fact]
    public async Task ListProductsSearchesAndFiltersDeterministically()
    {
        ProductSearchRequest request = new(
            Query: "premium",
            CategoryId: 1,
            ActiveOnly: true);

        IReadOnlyList<ProductDto> products = await _service.ListProductsAsync(request);

        ProductDto product = Assert.Single(products);
        Assert.Equal(2, product.Id);
    }

    [Fact]
    public async Task ListProductsExcludesDiscontinuedProductsByDefault()
    {
        IReadOnlyList<ProductDto> activeProducts =
            await _service.ListProductsAsync(new ProductSearchRequest());
        IReadOnlyList<ProductDto> allProducts =
            await _service.ListProductsAsync(new ProductSearchRequest(ActiveOnly: false));

        Assert.Equal([1, 2], activeProducts.Select(product => product.Id));
        Assert.Equal([1, 2, 3], allProducts.Select(product => product.Id));
    }

    [Fact]
    public async Task GetCategoriesReturnsProductOwnedCategories()
    {
        IReadOnlyList<ProductCategoryDto> categories = await _service.GetCategoriesAsync();

        Assert.Equal(["Hardware", "Office"], categories.Select(category => category.Name));
    }

    [Fact]
    public async Task AvailabilityReturnsCurrentPriceAndStockDecision()
    {
        ProductAvailabilityDto? available = await _service.GetAvailabilityAsync(1, 10);
        ProductAvailabilityDto? outOfStock = await _service.GetAvailabilityAsync(2, 1);
        ProductAvailabilityDto? discontinued = await _service.GetAvailabilityAsync(3, 1);

        Assert.NotNull(available);
        Assert.Equal(12.50m, available.CurrentUnitPrice);
        Assert.True(available.IsAvailable);
        Assert.False(outOfStock!.IsAvailable);
        Assert.False(discontinued!.IsAvailable);
    }

    [Fact]
    public async Task OperationsHonorCancellation()
    {
        using CancellationTokenSource cancellation = new();
        await cancellation.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.GetCategoriesAsync(cancellation.Token));
    }

    [Fact]
    public void ProductChangedHasValueSemanticsForIntegrationDelivery()
    {
        Guid eventId = Guid.Parse("b1f9ac17-5839-49b0-9f69-0cd93c44f0d5");
        DateTimeOffset occurredAt = DateTimeOffset.Parse("2026-01-02T03:04:05Z");
        ProductChanged first = new(
            eventId,
            1,
            "Widget",
            12.50m,
            25,
            true,
            false,
            1,
            occurredAt);
        ProductChanged second = first with { };

        Assert.Equal(first, second);
    }

    [Fact]
    public void CompositionEntryPointRegistersProductContract()
    {
        ServiceProvider provider = new ServiceCollection()
            .AddProductService()
            .BuildServiceProvider();

        IProductService service = provider.GetRequiredService<IProductService>();

        Assert.IsType<StubProductService>(service);
    }
}
