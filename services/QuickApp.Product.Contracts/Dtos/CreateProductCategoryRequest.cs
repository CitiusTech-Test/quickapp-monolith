namespace QuickApp.Product.Contracts.Dtos
{
    /// <summary>Request payload to create a new product category.</summary>
    public class CreateProductCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
