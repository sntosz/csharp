using ECommerceApi.Domain.Common;

namespace ECommerceApi.Domain.Entities;

/// <summary>
/// Entidade representando um Produto no e-commerce.
/// As propriedades possuem setters privados para garantir o encapsulamento de regras de domínio.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Construtor privado para o EF Core
    private Product() { }

    public Product(string name, string description, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));

        if (price < 0)
            throw new ArgumentException("O preço não pode ser negativo.", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("A quantidade em estoque não pode ser negativa.", nameof(stockQuantity));

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        IsActive = true;
    }

    public void UpdateInfo(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));

        if (price < 0)
            throw new ArgumentException("O preço não pode ser negativo.", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        Touch();
    }

    public Result DeductStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(new Error("Product.InvalidQuantity", "A quantidade a deduzir deve ser maior que zero."));

        if (StockQuantity < quantity)
            return Result.Failure(new Error("Product.InsufficientStock", $"Estoque insuficiente para o produto '{Name}'. Estoque disponível: {StockQuantity}."));

        StockQuantity -= quantity;
        Touch();
        return Result.Success();
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade a adicionar deve ser maior que zero.", nameof(quantity));

        StockQuantity += quantity;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }
}
