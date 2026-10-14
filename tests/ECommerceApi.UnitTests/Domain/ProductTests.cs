using ECommerceApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ECommerceApi.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void CreateProduct_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var product = new Product("Smartphone", "Top de linha", 2500.00m, 10);

        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be("Smartphone");
        product.Price.Should().Be(2500.00m);
        product.StockQuantity.Should().Be(10);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DeductStock_WhenSufficientStock_ShouldReduceQuantityAndSucceed()
    {
        // Arrange
        var product = new Product("Headset", "Bluetooth", 300.00m, 5);

        // Act
        var result = product.DeductStock(2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.StockQuantity.Should().Be(3);
    }

    [Fact]
    public void DeductStock_WhenInsufficientStock_ShouldReturnFailure()
    {
        // Arrange
        var product = new Product("Headset", "Bluetooth", 300.00m, 2);

        // Act
        var result = product.DeductStock(5);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Product.InsufficientStock");
        product.StockQuantity.Should().Be(2);
    }
}
