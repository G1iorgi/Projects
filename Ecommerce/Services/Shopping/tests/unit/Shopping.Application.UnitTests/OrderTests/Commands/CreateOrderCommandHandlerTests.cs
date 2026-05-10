using Moq;
using Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;
using Shopping.Domain;
using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Application.UnitTests.OrderTests.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Orders)
            .Returns(_orderRepoMock.Object);

        _handler = new CreateOrderCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCommandIsNull_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOrderItemsAreEmpty_ShouldThrowException()
    {
        // Arrange
        var command = new CreateOrderCommand(
            "user-1",
            100,
            Guid.NewGuid(),
            OrderStatus.Completed,
            []);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCreateOrderAndSave()
    {
        // Arrange
        var command = new CreateOrderCommand(
            "user-1",
            100,
            Guid.NewGuid(),
            OrderStatus.Completed,
            [
                new CreateOrderItemCommand(1, 50, 1),
                new CreateOrderItemCommand(2, 25, 2)
            ]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepoMock.Verify(x =>
            x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(result);
        Assert.Equal(command.TotalPrice, result.TotalPrice);
        Assert.Equal(command.TransactionId, result.TransactionId);
        Assert.Equal(command.Status, result.Status);
        Assert.Equal(2, result.OrderItems.Count);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldMapOrderItemsCorrectly()
    {
        // Arrange
        var command = new CreateOrderCommand(
            "user-1",
            50,
            Guid.NewGuid(),
            OrderStatus.Completed,
            [new CreateOrderItemCommand(1, 25, 2)]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var item = result.OrderItems.First();

        // Assert
        Assert.Equal(1, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(25, item.Price);
    }
}
