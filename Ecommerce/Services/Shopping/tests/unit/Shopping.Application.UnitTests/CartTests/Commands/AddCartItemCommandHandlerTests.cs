using Moq;
using SharedKernel.Exceptions.Product;
using Shopping.Application.Aggregates.CartAggregate.Commands.AddItem;
using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

namespace Shopping.Application.UnitTests.CartTests.Commands;

public class AddCartItemCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IProductApiProvider> _productApiMock = new();
    private readonly Mock<ICartRepository> _cartRepoMock = new();

    private readonly AddCartItemCommandHandler _handler;

    public AddCartItemCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(x => x.Carts)
            .Returns(_cartRepoMock.Object);

        _handler = new AddCartItemCommandHandler(
            _unitOfWorkMock.Object,
            _productApiMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCartDoesNotExist_ShouldCreateNewCartAndAddItem()
    {
        // Arrange
        var command = new AddCartItemCommand("user-1", "jwt", 1, 2);

        var product = new Product(
            Id: 1,
            Name: "Product A",
            Barcode: "barcode",
            Description: "desc",
            Price: 10,
            Image: null,
            Quantity: 5,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "cat");

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _cartRepoMock.Verify(x => x.Update(It.Is<Cart>(c =>
            c.UserId == command.UserId &&
            c.CartItems.Count == 1)), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCartExists_ShouldAddItemToExistingCart()
    {
        // Arrange
        var command = new AddCartItemCommand("user-1", "jwt", 1, 2);

        var cart = Cart.Create(command.UserId);

        var product = new Product(
            Id: 1,
            Name: "Product A",
            Barcode: "barcode",
            Description: "desc",
            Price: 10,
            Image: null,
            Quantity: 5,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "cat");

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Single(cart.CartItems);

        _cartRepoMock.Verify(x => x.Update(cart), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductIsNull_ShouldThrowProductNotFoundException()
    {
        // Arrange
        var command = new AddCartItemCommand("user-1", "jwt", 1, 2);

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenQuantityExceedsStock_ShouldThrowInsufficientQuantityException()
    {
        // Arrange
        var command = new AddCartItemCommand("user-1", "jwt", 1, 10);

        var product = new Product(
            Id: 1,
            Name: "Product A",
            Barcode: "barcode",
            Description: "desc",
            Price: 10,
            Image: null,
            Quantity: 5,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "cat");

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientProductQuantityException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallProductApiWithCorrectParameters()
    {
        // Arrange
        var command = new AddCartItemCommand("user-1", "jwt-token", 1, 2);

        var product = new Product(
            Id: 1,
            Name: "Product A",
            Barcode: "barcode",
            Description: "desc",
            Price: 10,
            Image: null,
            Quantity: 5,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "cat");

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _productApiMock.Verify(x =>
            x.GetProductByIdAsync("jwt-token", 1, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAddCorrectItemDataToCart()
    {
        // Arrange
        var command = new AddCartItemCommand("user-1", "jwt", 1, 3);

        var product = new Product(
            Id: 1,
            Name: "Test Product",
            Barcode: "barcode",
            Description: "desc",
            Price: 15,
            Image: null,
            Quantity: 10,
            CreateDate: DateTimeOffset.UtcNow,
            Status: ProductStatus.Enabled,
            CategoryId: 1,
            CategoryName: "cat");

        Cart? capturedCart = null;

        _productApiMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _cartRepoMock
            .Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        _cartRepoMock
            .Setup(x => x.Update(It.IsAny<Cart>()))
            .Callback<Cart>(c => capturedCart = c);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var item = capturedCart!.CartItems.First();

        Assert.Equal(1, item.ProductId);
        Assert.Equal("Test Product", item.ProductName);
        Assert.Equal(3, item.ProductQuantity);
        Assert.Equal(15, item.ProductPrice);
    }
}
