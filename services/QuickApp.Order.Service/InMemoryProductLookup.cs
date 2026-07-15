using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service;

/// <summary>
/// Stub <see cref="IProductLookup"/> backed by a fixed set of snapshots. Replace
/// with a Product service client adapter in a real deployment.
/// </summary>
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
