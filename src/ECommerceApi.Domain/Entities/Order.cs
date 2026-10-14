using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Enums;

namespace ECommerceApi.Domain.Entities;

/// <summary>
/// Agregado do Pedido. Contém a lista de itens, o status do pedido e o valor total.
/// </summary>
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();

    public string CustomerEmail { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public decimal TotalAmount => _items.Sum(item => item.TotalPrice);
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // EF Core constructor
    private Order() { }

    public Order(string customerEmail)
    {
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains('@'))
            throw new ArgumentException("E-mail do cliente inválido.", nameof(customerEmail));

        CustomerEmail = customerEmail;
        Status = OrderStatus.Pending;
    }

    public Result AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(new Error("Order.CannotModify", "Não é possível alterar itens de um pedido que não está pendente."));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            _items.Remove(existingItem);
            var updatedQuantity = existingItem.Quantity + quantity;
            _items.Add(new OrderItem(productId, productName, updatedQuantity, unitPrice));
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, quantity, unitPrice));
        }

        Touch();
        return Result.Success();
    }

    public Result Pay()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(new Error("Order.InvalidStatus", $"O pedido está no status '{Status}' e não pode ser pago."));

        if (!_items.Any())
            return Result.Failure(new Error("Order.EmptyOrder", "Não é possível pagar um pedido sem itens."));

        Status = OrderStatus.Paid;
        Touch();
        return Result.Success();
    }

    public Result Ship()
    {
        if (Status != OrderStatus.Paid)
            return Result.Failure(new Error("Order.InvalidStatus", "Apenas pedidos pagos podem ser enviados."));

        Status = OrderStatus.Shipped;
        Touch();
        return Result.Success();
    }

    public Result Deliver()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Failure(new Error("Order.InvalidStatus", "Apenas pedidos enviados podem ser entregues."));

        Status = OrderStatus.Delivered;
        Touch();
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Canceled)
            return Result.Failure(new Error("Order.CannotCancel", $"Pedido já finalizado com status '{Status}' e não pode ser cancelado."));

        Status = OrderStatus.Canceled;
        Touch();
        return Result.Success();
    }
}
