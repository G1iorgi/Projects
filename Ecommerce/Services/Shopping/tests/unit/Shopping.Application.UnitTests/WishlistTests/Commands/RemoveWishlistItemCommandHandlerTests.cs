using Moq;
using SharedKernel.Exceptions.Wishlist;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveItem;
using Shopping.Domain;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Application.UnitTests.WishlistTests.Commands;

public class RemoveWishlistItemCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IWishlistRepository> _wishlistRepoMock = new();
    private readonly RemoveWishlistItemCommandHandler _handler;

    public RemoveWishlistItemCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Wishlists)
            .Returns(_wishlistRepoMock.Object);

        _handler = new RemoveWishlistItemCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenWishlistDoesNotExist_ShouldThrowWishlistNotFoundException()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(1, "user-1");

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wishlist?)null);

        // Act & Assert
        await Assert.ThrowsAsync<WishlistNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenWishlistExists_ShouldRemoveItem()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(1, "user-1");
        var wishlist = Wishlist.Create(command.UserId);

        // Add items to the wishlist
        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.DoesNotContain(wishlist.WishlistItems, x => x.ProductId == command.ProductId);
        Assert.Single(wishlist.WishlistItems);

        _wishlistRepoMock.Verify(x => x.Update(wishlist), Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRemovingLastItem_ShouldLeaveEmptyWishlist()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(1, "user-1");
        var wishlist = Wishlist.Create(command.UserId);

        // Add only one item
        wishlist.AddItem(1, "Product A", 100);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(wishlist.WishlistItems);

        _wishlistRepoMock.Verify(x => x.Update(wishlist), Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductIdDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(999, "user-1");
        var wishlist = Wishlist.Create(command.UserId);

        // Add items to the wishlist
        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(wishlist);

        // Act & Assert
        await _handler.Handle(command, CancellationToken.None);

        // Both items should still be there
        Assert.Equal(2, wishlist.WishlistItems.Count());

        _wishlistRepoMock.Verify(x => x.Update(wishlist), Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }
}

