namespace ECommerceApi.Domain.Common;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Fornece identificador único (Guid) e auditoria simplificada (datas de criação/atualização).
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    public void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
