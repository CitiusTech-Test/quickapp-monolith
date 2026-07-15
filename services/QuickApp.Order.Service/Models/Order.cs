namespace QuickApp.Order.Service.Models;

public sealed class Order : BaseEntity
{
    public decimal Discount { get; set; }
    public string? Comments { get; set; }
    public string? CashierUserId { get; set; }
    public int CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public List<OrderDetail> OrderDetails { get; } = [];
    public decimal Subtotal => OrderDetails.Sum(item => item.Subtotal);
    public decimal LineItemDiscountTotal => OrderDetails.Sum(item => item.Discount);
    public decimal Total => Subtotal - LineItemDiscountTotal - Discount;
}
