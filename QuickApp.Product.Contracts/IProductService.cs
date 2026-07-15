namespace QuickApp.Product.Contracts;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDto>> ListProductsAsync(
        ProductSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductCategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<ProductAvailabilityDto?> GetAvailabilityAsync(
        int productId,
        int requestedQuantity,
        CancellationToken cancellationToken = default);
}
