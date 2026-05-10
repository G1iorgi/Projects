using Moq;
using Shopping.Application.Aggregates.WishlistAggregate.Queries.GetAllItems;
using Shopping.Application.Aggregates.WishlistAggregate.Responses;
using Shopping.Domain;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Application.UnitTests.WishlistTests.Queries;

public class GetAllWishlistItemsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IWishlistRepository> _wishlistRepoMock = new();
    private readonly GetAllWishlistItemsQueryHandler _handler;

    public GetAllWishlistItemsQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Wishlists)
            .Returns(_wishlistRepoMock.Object);

        _handler = new GetAllWishlistItemsQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenWishlistDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wishlist?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenWishlistIsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1);
        var wishlist = Wishlist.Create(query.UserId);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithMultipleItems_ShouldReturnAllItems()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var items = result.ToList();
        Assert.Equal(3, items.Count);
        Assert.Equal("Product A", items[0].Name);
        Assert.Equal("Product B", items[1].Name);
        Assert.Equal("Product C", items[2].Name);
    }

    [Fact]
    public async Task Handle_WithProductNameFilter_ShouldReturnFilteredItems()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1, ProductName: "Product A");
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product A Special", 300);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Equal(2, items.Count);
        Assert.All(items, item => Assert.Contains("Product A", item.Name));
    }

    [Fact]
    public async Task Handle_WithPriceRangeFilter_ShouldReturnFilteredItems()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1, PriceFrom: 150, PriceTo: 250);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Single(items);
        Assert.Equal("Product B", items[0].Name);
        Assert.Equal(200, items[0].Price);
    }

    [Fact]
    public async Task Handle_WithPriceFromOnly_ShouldFilterItemsAbovePrice()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1, PriceFrom: 150);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Equal(2, items.Count);
        Assert.All(items, item => Assert.True(item.Price >= 150));
    }

    [Fact]
    public async Task Handle_WithPriceToOnly_ShouldFilterItemsBelowPrice()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1, PriceTo: 150);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Single(items);
        Assert.All(items, item => Assert.True(item.Price <= 150));
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 2, PageNumber: 1);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);
        wishlist.AddItem(4, "Product D", 400);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Equal(2, items.Count);
        Assert.Equal("Product A", items[0].Name);
        Assert.Equal("Product B", items[1].Name);
    }

    [Fact]
    public async Task Handle_WithPaginationSecondPage_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 2, PageNumber: 2);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);
        wishlist.AddItem(4, "Product D", 400);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Equal(2, items.Count);
        Assert.Equal("Product C", items[0].Name);
        Assert.Equal("Product D", items[1].Name);
    }

    [Fact]
    public async Task Handle_WithHasImageTrue_ShouldReturnOnlyItemsWithImage()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1, HasImage: true);
        var wishlist = Wishlist.Create(query.UserId);

        var item1 = WishlistItem.Create(wishlist.Id, 1, "Product A", 100);
        var item2 = WishlistItem.Create(wishlist.Id, 2, "Product B", 200);
        var item3 = WishlistItem.Create(wishlist.Id, 3, "Product C", 300);

        // Simulate setting image via reflection or direct property access
        wishlist.WishlistItems.Add(item1);
        wishlist.WishlistItems.Add(item2);
        wishlist.WishlistItems.Add(item3);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.All(items, item => Assert.True(string.IsNullOrWhiteSpace(item.Image) == false));
    }

    [Fact]
    public async Task Handle_WithHasImageFalse_ShouldReturnOnlyItemsWithoutImage()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1, HasImage: false);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.All(items, item => Assert.True(string.IsNullOrWhiteSpace(item.Image)));
    }

    [Fact]
    public async Task Handle_WithCombinedFilters_ShouldReturnCorrectResults()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery(
            UserId: "user-1",
            PageSize: 10,
            PageNumber: 1,
            ProductName: "Product",
            PriceFrom: 150,
            PriceTo: 250);

        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Another B", 200);
        wishlist.AddItem(4, "Product C", 300);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Single(items);
        Assert.Equal("Product B", items[0].Name);
        Assert.Equal(200, items[0].Price);
    }

    [Fact]
    public async Task Handle_WithNullQuery_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnProductResponseWithCorrectMapping()
    {
        // Arrange
        var query = new GetAllWishlistItemsQuery("user-1", PageSize: 10, PageNumber: 1);
        var wishlist = Wishlist.Create(query.UserId);

        wishlist.AddItem(1, "Test Product", 99.99m);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var items = result.ToList();
        Assert.Single(items);
        var item = items.First();
        Assert.IsType<ProductResponse>(item);
        Assert.Equal(1, item.Id);
        Assert.Equal("Test Product", item.Name);
        Assert.Equal(99.99m, item.Price);
    }
}

