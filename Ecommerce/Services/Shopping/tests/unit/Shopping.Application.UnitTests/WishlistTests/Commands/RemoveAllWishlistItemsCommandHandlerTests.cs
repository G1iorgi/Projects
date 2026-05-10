using Moq;
using SharedKernel.Exceptions.Wishlist;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveAllItems;
using Shopping.Domain;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Application.UnitTests.WishlistTests.Commands;

public class RemoveAllWishlistItemsCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IWishlistRepository> _wishlistRepoMock = new();
    private readonly RemoveAllWishlistItemsCommandHandler _handler;

    public RemoveAllWishlistItemsCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Wishlists)
            .Returns(_wishlistRepoMock.Object);

        _handler = new RemoveAllWishlistItemsCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenWishlistDoesNotExist_ShouldThrowWishlistNotFoundException()
    {
        // Arrange
        var command = new RemoveAllWishlistItemsCommand("user-1");

        _wishlistRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wishlist?)null);

        // Act & Assert
        await Assert.ThrowsAsync<WishlistNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenWishlistExists_ShouldRemoveAllItems()
    {
        // Arrange
        var command = new RemoveAllWishlistItemsCommand("user-1");
        var wishlist = Wishlist.Create(command.UserId);

        // Add some items to the wishlist
        wishlist.AddItem(1, "Product A", 100);
        wishlist.AddItem(2, "Product B", 200);
        wishlist.AddItem(3, "Product C", 300);

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
    public async Task Handle_WhenWishlistIsEmpty_ShouldStillCompleteSuccessfully()
    {
        // Arrange
        var command = new RemoveAllWishlistItemsCommand("user-1");
        var wishlist = Wishlist.Create(command.UserId);

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
    public async Task Handle_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }
}

