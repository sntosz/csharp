using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Domain.Repositories;

/// <summary>
/// Contrato de repositório para acesso aos dados de produtos.
/// Desacopla a camada de aplicação da implementação de persistência.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Delete(Product product);
}
