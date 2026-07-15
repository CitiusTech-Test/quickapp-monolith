namespace QuickApp.Order.Service.Models;

public sealed class OrderDetail : BaseEntity
{
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public int OrderId { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public decimal Total => Subtotal - Discount;
}
