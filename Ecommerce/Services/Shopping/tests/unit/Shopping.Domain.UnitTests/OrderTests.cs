using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Domain.UnitTests;

public class OrderTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(totalPrice, order.TotalPrice);
        Assert.Equal(transactionId, order.TransactionId);
        Assert.Equal(status, order.Status);
        Assert.Single(order.OrderItems);
        Assert.Equal(orderItems[0], order.OrderItems.First());
    }

    [Fact]
    public void Create_WithNullUserId_ShouldThrow()
    {
        // Arrange
        string userId = null!;
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrow()
    {
        // Arrange
        const string userId = "";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithWhitespaceUserId_ShouldThrow()
    {
        // Arrange
        const string userId = "   ";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithZeroTotalPrice_ShouldThrow()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 0m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithNegativeTotalPrice_ShouldThrow()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = -149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithDefaultTransactionId_ShouldThrow()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.Empty;
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithNullOrderItems_ShouldThrow()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        List<OrderItem> orderItems = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithEmptyOrderItems_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 0m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>();

        // Act & Assert - Should throw because totalPrice is 0
        Assert.Throws<ArgumentException>(() =>
            Order.Create(userId, totalPrice, transactionId, status, orderItems));
    }

    [Fact]
    public void Create_WithMultipleOrderItems_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 339.90m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m),
            OrderItem.Create(101, 3, 49.99m),
            OrderItem.Create(102, 2, 19.99m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(3, order.OrderItems.Count);
    }

    [Fact]
    public void Create_WithRefundedStatus_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Refunded;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.Equal(OrderStatus.Refunded, order.Status);
    }

    [Fact]
    public void Create_WithCompletedStatus_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.Equal(OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void Create_ShouldSetOrderDateToUtcNow()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);
        var afterCreation = DateTimeOffset.UtcNow;

        // Assert
        Assert.InRange(order.OrderDate, beforeCreation.AddSeconds(-1), afterCreation.AddSeconds(1));
    }

    [Fact]
    public void Create_WithLargeTotalPrice_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = decimal.MaxValue;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, decimal.MaxValue)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.Equal(totalPrice, order.TotalPrice);
    }

    [Fact]
    public void Create_WithSmallTotalPrice_ShouldSucceed()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 0.01m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 1, 0.01m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.Equal(totalPrice, order.TotalPrice);
    }

    [Fact]
    public void OrderItems_ShouldNotBeNull()
    {
        // Arrange
        const string userId = "user-123";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.NotNull(order.OrderItems);
    }

    [Fact]
    public void Create_WithDifferentUserIds_ShouldHaveDifferentUsers()
    {
        // Arrange
        var transactionId1 = Guid.NewGuid();
        var transactionId2 = Guid.NewGuid();
        const decimal totalPrice = 149.95m;
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act
        var order1 = Order.Create("user-123", totalPrice, transactionId1, status, orderItems);
        var order2 = Order.Create("user-456", totalPrice, transactionId2, status, orderItems);

        // Assert
        Assert.NotEqual(order1.UserId, order2.UserId);
        Assert.NotEqual(order1.TransactionId, order2.TransactionId);
    }

    [Fact]
    public void Create_WithSpecialCharactersInUserId_ShouldSucceed()
    {
        // Arrange
        const string userId = "user@example.com-123_456";
        const decimal totalPrice = 149.95m;
        var transactionId = Guid.NewGuid();
        const OrderStatus status = OrderStatus.Completed;
        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(100, 5, 29.99m)
        };

        // Act
        var order = Order.Create(userId, totalPrice, transactionId, status, orderItems);

        // Assert
        Assert.Equal(userId, order.UserId);
    }
}

