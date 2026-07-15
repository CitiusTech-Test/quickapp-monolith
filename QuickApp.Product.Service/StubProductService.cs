using QuickApp.Product.Contracts;

namespace QuickApp.Product.Service;

public sealed class StubProductService : IProductService
{
    private static readonly ProductCategoryDto[] Categories =
    [
        new(1, "Hardware", "Physical products and accessories", "hardware"),
        new(2, "Office", "Office and stationery products", "office")
    ];

    private static readonly ProductDto[] Products =
    [
        new(
            1,
            "Widget",
            "Standard catalog widget",
            "widget",
            7.25m,
            12.50m,
            25,
            true,
            false,
            1,
            null),
        new(
            2,
            "Widget Pro",
            "Premium catalog widget",
            "widget-pro",
            15m,
            25m,
            0,
            true,
            false,
            1,
            1),
        new(
            3,
            "Retired Notebook",
            "Discontinued stationery item",
            "notebook",
            2m,
            4.50m,
            100,
            false,
            true,
            2,
            null)
    ];

    public Task<ProductDto?> GetProductByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ProductDto? product = Products.SingleOrDefault(product => product.Id == productId);
        return Task.FromResult(product);
    }

    public Task<IReadOnlyList<ProductDto>> ListProductsAsync(
        ProductSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(request);

        if (request.Skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Skip cannot be negative.");
        }

        if (request.Take is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Take must be between 1 and 100.");
        }

        IEnumerable<ProductDto> products = Products;

        if (request.ActiveOnly)
        {
            products = products.Where(product => product.IsActive && !product.IsDiscontinued);
        }

        if (request.CategoryId is not null)
        {
            products = products.Where(product => product.ProductCategoryId == request.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            string query = request.Query.Trim();
            products = products.Where(product =>
                product.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (product.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        IReadOnlyList<ProductDto> result = products
            .OrderBy(product => product.Id)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<ProductCategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ProductCategoryDto>>(Categories);
    }

    public Task<ProductAvailabilityDto?> GetAvailabilityAsync(
        int productId,
        int requestedQuantity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (requestedQuantity < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestedQuantity),
                "Requested quantity must be positive.");
        }

        ProductDto? product = Products.SingleOrDefault(product => product.Id == productId);
        if (product is null)
        {
            return Task.FromResult<ProductAvailabilityDto?>(null);
        }

        ProductAvailabilityDto availability = new(
            product.Id,
            product.Name,
            product.SellingPrice,
            requestedQuantity,
            product.UnitsInStock,
            product.IsActive &&
            !product.IsDiscontinued &&
            product.UnitsInStock >= requestedQuantity);

        return Task.FromResult<ProductAvailabilityDto?>(availability);
    }
}
