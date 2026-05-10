using Moq;
using SharedKernel.Exceptions.Cart;
using Shopping.Application.Aggregates.CartAggregate.Queries.GetItemsByUserId;
using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;

namespace Shopping.Application.UnitTests.CartTests.Queries;

public class GetCartItemsByUserIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly GetCartItemsByUserIdQueryHandler _handler;

    public GetCartItemsByUserIdQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Carts)
            .Returns(_cartRepoMock.Object);

        _handler = new GetCartItemsByUserIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldThrowEmptyCartException()
    {
        // Arrange
        var query = new GetCartItemsByUserIdQuery("user-1");

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EmptyCartException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldReturnAllItems()
    {
        // Arrange
        var query = new GetCartItemsByUserIdQuery("user-1");

        var cart = Cart.Create(query.UserId);
        cart.AddItem(1, "Product A", 2, 10);
        cart.AddItem(2, "Product B", 1, 20);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldMapCorrectly()
    {
        // Arrange
        var query = new GetCartItemsByUserIdQuery("user-1");

        var cart = Cart.Create(query.UserId);
        cart.AddItem(1, "Product A", 2, 10);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        var item = result.First();

        // Assert
        Assert.Equal(1, item.Id);
        Assert.Equal("Product A", item.Name);
        Assert.Equal(10, item.Price);
        Assert.Equal(2, item.Quantity);
    }
}
