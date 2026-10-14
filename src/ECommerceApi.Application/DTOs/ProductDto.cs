namespace ECommerceApi.Application.DTOs;

/// <summary>
/// DTO (Data Transfer Object) para representação de produtos exposta para a API.
/// Evita expor diretamente entidades de domínio no contrato público.
/// </summary>
public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
