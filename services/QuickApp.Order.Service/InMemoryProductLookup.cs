using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

public sealed class InMemoryProductLookup(IEnumerable<ProductSnapshotDto> products)
    : IProductLookup
{
    private readonly IReadOnlyDictionary<int, ProductSnapshotDto> _products =
        products.ToDictionary(product => product.ProductId);

    public Task<ProductSnapshotDto?> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _products.TryGetValue(productId, out var product);
        return Task.FromResult(product);
    }
}
