using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Domain.Repositories;

/// <summary>
/// Contrato de repositório para acesso aos dados de pedidos.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
}
