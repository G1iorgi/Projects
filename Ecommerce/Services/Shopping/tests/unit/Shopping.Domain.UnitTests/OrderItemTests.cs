using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Domain.UnitTests;

public class OrderItemTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        const int productId = 100;
        const int quantity = 5;
        const decimal unitPrice = 29.99m;

        // Act
        var orderItem = OrderItem.Create(productId, quantity, unitPrice);

        // Assert
        Assert.NotNull(orderItem);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
    }

    [Fact]
    public void Create_WithZeroProductId_ShouldThrow()
    {
        // Arrange
        const int productId = 0;
        const int quantity = 5;
        const decimal unitPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OrderItem.Create(productId, quantity, unitPrice));
    }

    [Fact]
    public void Create_WithNegativeProductId_ShouldThrow()
    {
        // Arrange
        const int productId = -1;
        const int quantity = 5;
        const decimal unitPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OrderItem.Create(productId, quantity, unitPrice));
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldThrow()
    {
        // Arrange
        const int productId = 100;
        const int quantity = 0;
        const decimal unitPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OrderItem.Create(productId, quantity, unitPrice));
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldThrow()
    {
        // Arrange
        const int productId = 100;
        const int quantity = -5;
        const decimal unitPrice = 29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OrderItem.Create(productId, quantity, unitPrice));
    }

    [Fact]
    public void Create_WithZeroUnitPrice_ShouldThrow()
    {
        // Arrange
        const int productId = 100;
        const int quantity = 5;
        const decimal unitPrice = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OrderItem.Create(productId, quantity, unitPrice));
    }

    [Fact]
    public void Create_WithNegativeUnitPrice_ShouldThrow()
    {
        // Arrange
        const int productId = 100;
        const int quantity = 5;
        const decimal unitPrice = -29.99m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OrderItem.Create(productId, quantity, unitPrice));
    }

    [Fact]
    public void Create_WithLargeValues_ShouldSucceed()
    {
        // Arrange
        const int productId = int.MaxValue;
        const int quantity = int.MaxValue;
        const decimal unitPrice = decimal.MaxValue;

        // Act
        var orderItem = OrderItem.Create(productId, quantity, unitPrice);

        // Assert
        Assert.NotNull(orderItem);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
    }

    [Fact]
    public void Create_WithSmallDecimalPrice_ShouldSucceed()
    {
        // Arrange
        const int productId = 100;
        const int quantity = 5;
        const decimal unitPrice = 0.01m;

        // Act
        var orderItem = OrderItem.Create(productId, quantity, unitPrice);

        // Assert
        Assert.NotNull(orderItem);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
    }
}

