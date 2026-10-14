namespace ECommerceApi.Domain.Repositories;

/// <summary>
/// Pattern Unit of Work: Garante a consistência transacional ao salvar alterações de múltiplos repositórios.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
