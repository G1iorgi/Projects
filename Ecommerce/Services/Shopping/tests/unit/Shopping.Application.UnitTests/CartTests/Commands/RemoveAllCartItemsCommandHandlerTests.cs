using Moq;
using SharedKernel.Exceptions.Cart;
using Shopping.Application.Aggregates.CartAggregate.Commands.RemoveAllItems;
using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;

namespace Shopping.Application.UnitTests.CartTests.Commands;

public class RemoveAllCartItemsCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly RemoveAllCartItemsCommandHandler _handler;

    public RemoveAllCartItemsCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Carts)
            .Returns(_cartRepoMock.Object);
        _handler = new RemoveAllCartItemsCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldThrowCartNotFoundException()
    {
        // Arrange
        var command = new RemoveAllCartItemsCommand("user-1");
        _cartRepoMock.Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CartNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldRemoveAllItemsAndSave()
    {
        // Arrange
        var command = new RemoveAllCartItemsCommand("user-1");
        var cart = Cart.Create(command.UserId);
        cart.AddItem(1, "Product A", 2, 10);
        cart.AddItem(2, "Product B", 1, 20);
        _cartRepoMock.Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Empty(cart.CartItems);
        _cartRepoMock.Verify(x => x.Update(cart), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
