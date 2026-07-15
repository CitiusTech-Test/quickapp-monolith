namespace QuickApp.Order.Contracts;

/// <summary>
/// Lifecycle state owned by the Order domain. Only the states required by the
/// POC are modelled; a production workflow (cancellation, fulfilment, refunds)
/// requires manual architecture review before it is added.
/// </summary>
public enum OrderStatus
{
    Placed = 0
}
