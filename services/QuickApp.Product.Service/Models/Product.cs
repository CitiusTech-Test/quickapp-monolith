namespace QuickApp.Product.Service.Models
{
    /// <summary>
    /// Product domain model owned by the Product service.
    /// Mirrors the monolith <c>Product</c> EXCEPT the <c>ICollection&lt;OrderDetail&gt; OrderDetails</c>
    /// navigation, which is removed to break coupling with the Order service.
    /// The self-referencing Parent/Children and the ProductCategory relationship are kept
    /// because both are owned by this service.
    /// </summary>
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsDiscontinued { get; set; }

        public int? ParentId { get; set; }
        public Product? Parent { get; set; }

        public int ProductCategoryId { get; set; }
        public ProductCategory? ProductCategory { get; set; }

        public ICollection<Product> Children { get; } = [];
    }
}
