namespace QuickApp.Order.Contracts;

public interface IProductLookup
{
    Task<ProductSnapshotDto?> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default);
}
