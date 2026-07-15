namespace QuickApp.Product.Contracts;

public sealed record ProductDto(
    int Id,
    string Name,
    string? Description,
    string? Icon,
    decimal BuyingPrice,
    decimal SellingPrice,
    int UnitsInStock,
    bool IsActive,
    bool IsDiscontinued,
    int ProductCategoryId,
    int? ParentId);

public sealed record ProductCategoryDto(
    int Id,
    string Name,
    string? Description,
    string? Icon);

public sealed record ProductSearchRequest(
    string? Query = null,
    int? CategoryId = null,
    bool ActiveOnly = true,
    int Skip = 0,
    int Take = 50);

public sealed record ProductAvailabilityDto(
    int ProductId,
    string ProductName,
    decimal CurrentUnitPrice,
    int RequestedQuantity,
    int UnitsInStock,
    bool IsAvailable);
