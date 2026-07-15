namespace QuickApp.Product.Contracts;

public sealed record ProductChanged(
    Guid EventId,
    int ProductId,
    string Name,
    decimal SellingPrice,
    int UnitsInStock,
    bool IsActive,
    bool IsDiscontinued,
    int ProductCategoryId,
    DateTimeOffset OccurredAtUtc);
