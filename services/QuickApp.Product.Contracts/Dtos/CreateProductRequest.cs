namespace QuickApp.Product.Contracts.Dtos
{
    /// <summary>Request payload to create a new product.</summary>
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDiscontinued { get; set; }
        public int? ParentId { get; set; }
        public int ProductCategoryId { get; set; }
    }
}
