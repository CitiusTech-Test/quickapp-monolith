using QuickApp.Product.Contracts;
using QuickApp.Product.Contracts.Dtos;
using QuickApp.Product.Service.Models;

namespace QuickApp.Product.Service
{
    /// <summary>
    /// Deterministic in-memory stub implementation of <see cref="IProductService"/> for the
    /// decomposition POC. Not thread-safe and not persistent — intended for tests and local demos.
    /// TODO: replace with an EF Core / repository-backed implementation once the service is stood up.
    /// </summary>
    public class InMemoryProductService : IProductService
    {
        private readonly Dictionary<int, Models.Product> _products = new();
        private readonly Dictionary<int, ProductCategory> _categories = new();
        private int _nextProductId = 1;
        private int _nextCategoryId = 1;

        // ---- Products ----

        public Task<IReadOnlyList<ProductDto>> GetProductsAsync()
        {
            IReadOnlyList<ProductDto> result = _products.Values
                .OrderBy(p => p.Id)
                .Select(ToDto)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<ProductDto?> GetProductByIdAsync(int id)
        {
            return Task.FromResult(_products.TryGetValue(id, out var product) ? ToDto(product) : null);
        }

        public Task<IReadOnlyList<ProductDto>> GetChildProductsAsync(int parentId)
        {
            IReadOnlyList<ProductDto> result = _products.Values
                .Where(p => p.ParentId == parentId)
                .OrderBy(p => p.Id)
                .Select(ToDto)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<ProductDto> CreateProductAsync(CreateProductRequest request)
        {
            ValidateCategoryExists(request.ProductCategoryId);
            ValidateParentExists(request.ParentId);

            var now = DateTime.UtcNow;
            var product = new Models.Product
            {
                Id = _nextProductId++,
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                BuyingPrice = request.BuyingPrice,
                SellingPrice = request.SellingPrice,
                UnitsInStock = request.UnitsInStock,
                IsActive = request.IsActive,
                IsDiscontinued = request.IsDiscontinued,
                ParentId = request.ParentId,
                ProductCategoryId = request.ProductCategoryId,
                CreatedDate = now,
                UpdatedDate = now
            };

            _products[product.Id] = product;
            return Task.FromResult(ToDto(product));
        }

        public Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            if (!_products.TryGetValue(id, out var product))
                return Task.FromResult<ProductDto?>(null);

            ValidateCategoryExists(request.ProductCategoryId);
            ValidateParentExists(request.ParentId);
            if (request.ParentId == id)
                throw new InvalidOperationException("A product cannot be its own parent.");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Icon = request.Icon;
            product.BuyingPrice = request.BuyingPrice;
            product.SellingPrice = request.SellingPrice;
            product.UnitsInStock = request.UnitsInStock;
            product.IsActive = request.IsActive;
            product.IsDiscontinued = request.IsDiscontinued;
            product.ParentId = request.ParentId;
            product.ProductCategoryId = request.ProductCategoryId;
            product.UpdatedDate = DateTime.UtcNow;

            return Task.FromResult<ProductDto?>(ToDto(product));
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            if (!_products.Remove(id))
                return Task.FromResult(false);

            // Detach any children so they are not left pointing at a now-missing parent.
            foreach (var child in _products.Values.Where(p => p.ParentId == id))
            {
                child.ParentId = null;
                child.Parent = null;
                child.UpdatedDate = DateTime.UtcNow;
            }

            return Task.FromResult(true);
        }

        public Task<ProductDto?> AdjustStockAsync(int id, int delta)
        {
            if (!_products.TryGetValue(id, out var product))
                return Task.FromResult<ProductDto?>(null);

            var newStock = product.UnitsInStock + delta;
            if (newStock < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            product.UnitsInStock = newStock;
            product.UpdatedDate = DateTime.UtcNow;
            return Task.FromResult<ProductDto?>(ToDto(product));
        }

        // ---- Categories ----

        public Task<IReadOnlyList<ProductCategoryDto>> GetCategoriesAsync()
        {
            IReadOnlyList<ProductCategoryDto> result = _categories.Values
                .OrderBy(c => c.Id)
                .Select(ToDto)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<ProductCategoryDto?> GetCategoryByIdAsync(int id)
        {
            return Task.FromResult(_categories.TryGetValue(id, out var category) ? ToDto(category) : null);
        }

        public Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequest request)
        {
            var now = DateTime.UtcNow;
            var category = new ProductCategory
            {
                Id = _nextCategoryId++,
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                CreatedDate = now,
                UpdatedDate = now
            };

            _categories[category.Id] = category;
            return Task.FromResult(ToDto(category));
        }

        // ---- Helpers ----

        private void ValidateCategoryExists(int categoryId)
        {
            if (!_categories.ContainsKey(categoryId))
                throw new InvalidOperationException($"Product category {categoryId} does not exist.");
        }

        private void ValidateParentExists(int? parentId)
        {
            if (parentId.HasValue && !_products.ContainsKey(parentId.Value))
                throw new InvalidOperationException($"Parent product {parentId.Value} does not exist.");
        }

        private static ProductDto ToDto(Models.Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Icon = p.Icon,
            BuyingPrice = p.BuyingPrice,
            SellingPrice = p.SellingPrice,
            UnitsInStock = p.UnitsInStock,
            IsActive = p.IsActive,
            IsDiscontinued = p.IsDiscontinued,
            ParentId = p.ParentId,
            ProductCategoryId = p.ProductCategoryId
        };

        private static ProductCategoryDto ToDto(ProductCategory c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Icon = c.Icon
        };
    }
}
