namespace ECommerceApi.Domain.Common;

/// <summary>
/// Representa um erro no domínio/aplicação com um código identificador e uma mensagem descritiva.
/// </summary>
public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Valor nulo foi fornecido.");
}
