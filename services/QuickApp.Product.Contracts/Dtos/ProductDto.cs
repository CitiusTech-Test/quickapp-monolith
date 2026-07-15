namespace QuickApp.Product.Contracts.Dtos
{
    /// <summary>
    /// Plain POCO representation of a product. No EF/Identity dependencies.
    /// The Order navigation (OrderDetails) present on the monolith entity is intentionally
    /// dropped here to break cross-service coupling with the Order service.
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsDiscontinued { get; set; }

        /// <summary>Self-referencing parent product id (owned by this service).</summary>
        public int? ParentId { get; set; }

        /// <summary>Category this product belongs to (owned by this service).</summary>
        public int ProductCategoryId { get; set; }
    }
}
