using Moq;
using Shopping.Application.Aggregates.CartAggregate.Queries.GetAllItems;
using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;

namespace Shopping.Application.UnitTests.CartTests.Queries;

public class GetAllCartItemsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly GetAllCartItemsQueryHandler _handler;

    public GetAllCartItemsQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Carts)
            .Returns(_cartRepoMock.Object);

        _handler = new GetAllCartItemsQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllCartItemsQuery("user-1", 10, 1);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldReturnAllItems()
    {
        // Arrange
        var query = new GetAllCartItemsQuery("user-1", 10, 1);

        var cart = Cart.Create(query.UserId);
        cart.AddItem(1, "Product A", 2, 10);
        cart.AddItem(2, "Product B", 1, 20);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WithProductNameFilter_ShouldReturnFilteredItems()
    {
        // Arrange
        var query = new GetAllCartItemsQuery("user-1", 10, 1, ProductName: "A");

        var cart = Cart.Create(query.UserId);
        cart.AddItem(1, "Apple", 2, 10);
        cart.AddItem(2, "Banana", 1, 20);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Apple", result[0].Name);
    }

    [Fact]
    public async Task Handle_WithPriceRange_ShouldReturnFilteredItems()
    {
        // Arrange
        var query = new GetAllCartItemsQuery("user-1", 10, 1, PriceFrom: 15, PriceTo: 25);

        var cart = Cart.Create(query.UserId);
        cart.AddItem(1, "Cheap", 1, 10);
        cart.AddItem(2, "Expensive", 1, 20);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Expensive", result[0].Name);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetAllCartItemsQuery("user-1", 1, 2);

        var cart = Cart.Create(query.UserId);
        cart.AddItem(1, "First", 1, 10);
        cart.AddItem(2, "Second", 1, 20);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Second", result[0].Name);
    }
}
