namespace QuickApp.Order.Contracts;

/// <summary>
/// Primary port of the Order domain. Representative operations only: place an
/// order, get an order by id, and list orders.
/// </summary>
public interface IOrdersService
{
    Task<OrderDto> PlaceOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderDto?> GetOrderByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderDto>> ListOrdersAsync(
        CancellationToken cancellationToken = default);
}
