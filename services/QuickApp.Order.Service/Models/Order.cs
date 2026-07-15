using QuickApp.Order.Contracts;

namespace QuickApp.Order.Service.Models;

/// <summary>
/// Order aggregate root owned by the service. References external domains only
/// through scalar ids and immutable snapshot fields.
/// </summary>
public sealed class Order : BaseEntity
{
    public OrderStatus Status { get; set; } = OrderStatus.Placed;
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
