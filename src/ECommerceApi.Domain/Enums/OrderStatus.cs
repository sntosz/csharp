namespace ECommerceApi.Domain.Enums;

/// <summary>
/// Status do ciclo de vida de um Pedido no sistema.
/// </summary>
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Shipped = 3,
    Delivered = 4,
    Canceled = 5
}
