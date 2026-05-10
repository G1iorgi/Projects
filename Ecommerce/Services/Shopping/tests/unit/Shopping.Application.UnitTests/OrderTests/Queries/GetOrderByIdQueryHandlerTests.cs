using Moq;
using SharedKernel.Exceptions.Order;
using Shopping.Application.Aggregates.OrderAggregate.Queries.GetOrderById;
using Shopping.Domain;
using Shopping.Domain.Aggregates.OrderAggregate;

namespace Shopping.Application.UnitTests.OrderTests.Queries;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Orders)
            .Returns(_orderRepoMock.Object);

        _handler = new GetOrderByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ShouldThrowOrderNotFoundException()
    {
        // Arrange
        var query = new GetOrderByIdQuery(1);

        _orderRepoMock
            .Setup(x => x.GetByIdAsync(query.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var query = new GetOrderByIdQuery(1);

        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(1, 2, 10),
            OrderItem.Create(2, 1, 20)
        };

        var order = Order.Create(
            "user-1",
            40,
            Guid.NewGuid(),
            OrderStatus.Completed,
            orderItems);

        _orderRepoMock
            .Setup(x => x.GetByIdAsync(query.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.OrderId);
        Assert.Equal(order.TotalPrice, result.TotalPrice);
        Assert.Equal(order.Status, result.Status);
        Assert.Equal(2, result.OrderItems.Count);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ShouldMapItemsCorrectly()
    {
        // Arrange
        var query = new GetOrderByIdQuery(1);

        var orderItems = new List<OrderItem>
        {
            OrderItem.Create(5, 3, 15)
        };

        var order = Order.Create(
            "user-1",
            45,
            Guid.NewGuid(),
            OrderStatus.Completed,
            orderItems);

        _orderRepoMock
            .Setup(x => x.GetByIdAsync(query.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        var item = result.OrderItems.First();

        // Assert
        Assert.Equal(5, item.ProductId);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(15, item.Price);
    }
}
