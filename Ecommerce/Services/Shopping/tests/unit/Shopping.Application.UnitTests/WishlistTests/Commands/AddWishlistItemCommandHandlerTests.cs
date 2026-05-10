using Moq;
using SharedKernel.Exceptions.Product;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.AddItem;
using Shopping.Domain;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Application.UnitTests.WishlistTests.Commands;

public class AddWishlistItemCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IWishlistRepository> _wishlistRepoMock = new();
    private readonly Mock<IProductApiProvider> _productApiMock = new();
    private readonly AddWishlistItemCommandHandler _handler;

    public AddWishlistItemCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Wishlists)
            .Returns(_wishlistRepoMock.Object);

        _handler = new AddWishlistItemCommandHandler(
            _unitOfWorkMock.Object,
            _productApiMock.Object);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var command = new AddWishlistItemCommand("user-1", "jwt", 1);

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenWishlistDoesNotExist_ShouldCreateWishlistAndAddItem()
    {
        // Arrange
        var command = new AddWishlistItemCommand("user-1", "jwt", 1);

        var product = new Product(
            Id: 1,
            Name: "Product A",
            Barcode: "BARCODE001",
            Description: "Test Product",
            Price: 100,
            Image: null,
            Quantity: 50,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "Test Category");

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wishlist?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _wishlistRepoMock.Verify(x =>
            x.Update(It.Is<Wishlist>(w =>
                w.WishlistItems.Any(i => i.ProductId == command.ProductId))),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenWishlistExists_ShouldAddItem()
    {
        // Arrange
        var command = new AddWishlistItemCommand("user-1", "jwt", 1);

        var product = new Product(
            Id: 1,
            Name: "Product A",
            Barcode: "BARCODE001",
            Description: "Test Product",
            Price: 100,
            Image: null,
            Quantity: 50,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "Test Category");

        var wishlist = Wishlist.Create(command.UserId);

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Contains(wishlist.WishlistItems, x => x.ProductId == command.ProductId);

        _wishlistRepoMock.Verify(x => x.Update(wishlist), Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
