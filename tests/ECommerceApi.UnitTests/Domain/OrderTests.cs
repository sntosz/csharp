using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ECommerceApi.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void AddItem_ShouldCalculateTotalAmountCorrectly()
    {
        // Arrange
        var order = new Order("cliente@email.com");
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        // Act
        order.AddItem(productId1, "Produto A", 2, 50.00m); // 100
        order.AddItem(productId2, "Produto B", 1, 30.00m); // 30

        // Assert
        order.TotalAmount.Should().Be(130.00m);
        order.Items.Should().HaveCount(2);
    }

    [Fact]
    public void Pay_WhenOrderIsPendingAndHasItems_ShouldChangeStatusToPaid()
    {
        // Arrange
        var order = new Order("cliente@email.com");
        order.AddItem(Guid.NewGuid(), "Produto", 1, 100.00m);

        // Act
        var result = order.Pay();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void Pay_WhenOrderHasNoItems_ShouldReturnFailure()
    {
        // Arrange
        var order = new Order("cliente@email.com");

        // Act
        var result = order.Pay();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.EmptyOrder");
    }
}
