namespace QuickApp.Product.Service.Models
{
    /// <summary>
    /// Product category domain model owned by the Product service.
    /// Mirrors the monolith <c>ProductCategory</c> but without EF concerns.
    /// </summary>
    public class ProductCategory : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }

        public ICollection<Product> Products { get; } = [];
    }
}
