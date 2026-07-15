using QuickApp.Product.Contracts.Dtos;

namespace QuickApp.Product.Contracts
{
    /// <summary>
    /// CRUD contract for the standalone Product service. This is the extracted and fleshed-out
    /// version of the (empty) <c>IProductService</c> in the monolith
    /// (<c>QuickApp.Core/Services/Shop/Interfaces/IProductService.cs</c>).
    /// All operations are async and communicate exclusively through DTOs.
    /// </summary>
    public interface IProductService
    {
        // Products
        Task<IReadOnlyList<ProductDto>> GetProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IReadOnlyList<ProductDto>> GetChildProductsAsync(int parentId);
        Task<ProductDto> CreateProductAsync(CreateProductRequest request);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);

        /// <summary>Adjusts a product's stock by <paramref name="delta"/> (positive or negative).</summary>
        /// <returns>The updated product, or null if it does not exist.</returns>
        Task<ProductDto?> AdjustStockAsync(int id, int delta);

        // Categories
        Task<IReadOnlyList<ProductCategoryDto>> GetCategoriesAsync();
        Task<ProductCategoryDto?> GetCategoryByIdAsync(int id);
        Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequest request);
    }
}
