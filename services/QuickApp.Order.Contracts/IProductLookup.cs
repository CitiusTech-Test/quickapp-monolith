namespace QuickApp.Order.Contracts;

/// <summary>
/// Local port for resolving product price and availability. Belongs to the Order
/// boundary and must be implemented by an adapter (e.g. a Product service
/// client); it must not reference another service's implementation.
/// </summary>
public interface IProductLookup
{
    Task<ProductSnapshotDto?> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default);
}
