namespace QuickApp.Order.Contracts;

public interface IOrdersService
{
    Task<OrderDto> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderDto?> GetOrderByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderDto>> ListOrdersAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderDto>> ListOrdersByCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default);
}
