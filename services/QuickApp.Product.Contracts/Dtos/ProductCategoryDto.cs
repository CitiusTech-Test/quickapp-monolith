namespace QuickApp.Product.Contracts.Dtos
{
    /// <summary>
    /// Plain POCO representation of a product category. No EF/Identity dependencies.
    /// </summary>
    public class ProductCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
