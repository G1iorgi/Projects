using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Infrastructure.DbContexts;
using Shopping.Infrastructure.Repositories;

namespace Shopping.Infrastructure.UnitTests.Repositories;

public class OrderRepositoryTests : IDisposable
{
    private readonly ShoppingDbContextMaster _dbContext;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ShoppingDbContextMaster>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ShoppingDbContextMaster(options);
        _repository = new OrderRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetByTransactionIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        const string userId = "user-123";
        const decimal totalPrice = 100.00m;
        var orderItems = new List<OrderItem>();

        var order = Order.Create(userId, totalPrice, transactionId, OrderStatus.Completed, orderItems);
        await _repository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(totalPrice, result.TotalPrice);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentTransactionId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByTransactionIdAsync(nonExistentTransactionId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_WithMultipleOrders_ShouldReturnCorrectOrder()
    {
        // Arrange
        var transactionId1 = Guid.NewGuid();
        var transactionId2 = Guid.NewGuid();
        var transactionId3 = Guid.NewGuid();

        var order1 = Order.Create("user-1", 100.00m, transactionId1, OrderStatus.Completed, new List<OrderItem>());
        var order2 = Order.Create("user-2", 200.00m, transactionId2, OrderStatus.Completed, new List<OrderItem>());
        var order3 = Order.Create("user-3", 300.00m, transactionId3, OrderStatus.Refunded, new List<OrderItem>());

        await _repository.CreateAsync(order1);
        await _repository.CreateAsync(order2);
        await _repository.CreateAsync(order3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId2, result.TransactionId);
        Assert.Equal("user-2", result.UserId);
        Assert.Equal(200.00m, result.TotalPrice);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_WithDefaultCancellationToken_ShouldWork()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        const string userId = "user-456";
        const decimal totalPrice = 250.50m;
        var order = Order.Create(userId, totalPrice, transactionId, OrderStatus.Completed, new List<OrderItem>());
        await _repository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act - Using default CancellationToken (not explicitly provided)
        var result = await _repository.GetByTransactionIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_WithEmptyGuid_ShouldReturnNull()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = await _repository.GetByTransactionIdAsync(emptyGuid);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_ShouldReturnOrderWithOrderItems()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        const string userId = "user-789";
        const decimal totalPrice = 500.00m;

        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(1, 2, 100.00m),
            OrderItem.Create(2, 3, 100.00m)
        };

        var order = Order.Create(userId, totalPrice, transactionId, OrderStatus.Completed, orderItems);
        await _repository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.OrderItems.Count);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_ShouldReturnOrderWithCorrectStatus()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        const string userId = "user-status-test";
        const decimal totalPrice = 150.00m;

        var order = Order.Create(userId, totalPrice, transactionId, OrderStatus.Refunded, new List<OrderItem>());
        await _repository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Refunded, result.Status);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_ShouldReturnOrderWithCorrectOrderDate()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        const string userId = "user-date-test";
        const decimal totalPrice = 200.00m;
        var beforeCreation = DateTimeOffset.UtcNow;

        var order = Order.Create(userId, totalPrice, transactionId, OrderStatus.Completed, new List<OrderItem>());
        await _repository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        var afterCreation = DateTimeOffset.UtcNow;

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.OrderDate >= beforeCreation && result.OrderDate <= afterCreation);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_WithDifferentTransactionIds_ShouldNotReturnWrongOrder()
    {
        // Arrange
        var transactionId1 = Guid.NewGuid();
        var transactionId2 = Guid.NewGuid();

        var order1 = Order.Create("user-1", 100.00m, transactionId1, OrderStatus.Completed, new List<OrderItem>());
        var order2 = Order.Create("user-2", 200.00m, transactionId2, OrderStatus.Completed, new List<OrderItem>());

        await _repository.CreateAsync(order1);
        await _repository.CreateAsync(order2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId1, result.TransactionId);
        Assert.NotEqual(transactionId2, result.TransactionId);
    }

    [Fact]
    public async Task GetByTransactionIdAsync_ShouldReturnOrderWithCorrectIdSet()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        const string userId = "user-id-test";
        const decimal totalPrice = 175.50m;

        var order = Order.Create(userId, totalPrice, transactionId, OrderStatus.Completed, new List<OrderItem>());
        await _repository.CreateAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTransactionIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }
}
