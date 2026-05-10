using Moq;
using Payment.Application.Aggregates.PaymentAggregate.Commands.PayFromCart;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider.DTOs;
using Payment.Domain.Aggregates.PaymentAggregate;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using SharedKernel.Contracts.Abstractions;
using SharedKernel.Contracts.Events;
using SharedKernel.Exceptions.Cart;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.UnitTests.Commands;

public class PayFromCartCommandHandlerTests
{
    private readonly Mock<IPaymentApiProvider> _paymentApiProviderMock = new();
    private readonly Mock<ICartApiProvider> _cartApiProviderMock = new();
    private readonly Mock<IProductApiProvider> _productApiProviderMock = new();
    private readonly Mock<IEventBus> _eventBusMock = new();

    private readonly PayFromCartCommandHandler _handler;

    public PayFromCartCommandHandlerTests()
    {
        _handler = new PayFromCartCommandHandler(
            _paymentApiProviderMock.Object,
            _cartApiProviderMock.Object,
            _productApiProviderMock.Object,
            _eventBusMock.Object);

        _eventBusMock
            .Setup(x => x.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _eventBusMock
            .Setup(x => x.PublishAsync(It.IsAny<ProductsQuantitiesDecreasedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _eventBusMock
            .Setup(x => x.PublishAsync(It.IsAny<CartEmptiedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_WithValidCart_ShouldSucceedAndPublishAllEvents()
    {
        var command = new PayFromCartCommand(
            "user-1", "jwt", "1234567812345678", "12/25", "123", CurrencyType.USD);

        var cartItems = new List<CartItem>
        {
            new()
            {
                Id = 1,
                Quantity = 2
            },
            new()
            {
                Id = 2,
                Quantity = 1
            }
        };

        var products = new List<Product>
        {
            new(1, "A", "B", "D", 50m, null, 10, DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "C"),
            new(2, "B", "B", "D", 100m, null, 10, DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "C")
        };

        var balance = new Balance(10000m);
        var transactionId = Guid.NewGuid();

        _cartApiProviderMock
            .Setup(x => x.GetCartItemsByUserId(command.Jwt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        _productApiProviderMock
            .Setup(x => x.GetProductsByIdsAsync(command.Jwt, It.IsAny<GetProductsByIdsDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        _paymentApiProviderMock
            .Setup(x => x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CurrencyType>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);

        await _handler.Handle(command, CancellationToken.None);

        _eventBusMock.Verify(x =>
            x.PublishAsync(It.Is<OrderCreatedEvent>(e =>
                e.UserId == command.UserId &&
                e.TransactionId == transactionId &&
                e.OrderItems.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _eventBusMock.Verify(x =>
            x.PublishAsync(It.IsAny<ProductsQuantitiesDecreasedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _eventBusMock.Verify(x =>
            x.PublishAsync(It.Is<CartEmptiedEvent>(e =>
                e.UserId == command.UserId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ShouldThrow()
    {
        var command = new PayFromCartCommand(
            "user-1", "jwt", "123", "12/25", "123", CurrencyType.USD);

        _cartApiProviderMock
            .Setup(x => x.GetCartItemsByUserId(command.Jwt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CartItem>());

        await Assert.ThrowsAsync<EmptyCartException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _eventBusMock.Verify(x =>
            x.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductMissing_ShouldThrow()
    {
        var command = new PayFromCartCommand(
            "user-1", "jwt", "123", "12/25", "123", CurrencyType.USD);

        var cartItems = new List<CartItem>
        {
            new()
            {
                Id = 1,
                Quantity = 2
            }
        };

        _cartApiProviderMock
            .Setup(x => x.GetCartItemsByUserId(command.Jwt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        _productApiProviderMock
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<string>(), It.IsAny<GetProductsByIdsDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        await Assert.ThrowsAsync<CartItemsNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenInsufficientProductQuantity_ShouldThrow()
    {
        var command = new PayFromCartCommand(
            "user-1", "jwt", "123", "12/25", "123", CurrencyType.USD);

        var cartItems = new List<CartItem>
        {
            new()
            {
                Id = 1,
                Quantity = 2
            }
        };

        var products = new List<Product>
        {
            new(1, "A", "B", "D", 50m, null, 1,
                DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "C")
        };

        _cartApiProviderMock
            .Setup(x => x.GetCartItemsByUserId(command.Jwt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        _productApiProviderMock
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<string>(), It.IsAny<GetProductsByIdsDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        await Assert.ThrowsAsync<InsufficientProductQuantityException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenBalanceNotFound_ShouldThrow()
    {
        var command = new PayFromCartCommand(
            "user-1", "jwt", "123", "12/25", "123", CurrencyType.USD);

        var cartItems = new List<CartItem>
        {
            new()
            {
                Id = 1,
                Quantity = 2
            }
        };

        var products = new List<Product>
        {
            new(1, "A", "B", "D", 50m, null, 10,
                DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "C")
        };

        _cartApiProviderMock
            .Setup(x => x.GetCartItemsByUserId(command.Jwt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        _productApiProviderMock
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<string>(), It.IsAny<GetProductsByIdsDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Balance?)null);

        await Assert.ThrowsAsync<BalanceNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPaymentFails_ShouldThrow()
    {
        var command = new PayFromCartCommand(
            "user-1", "jwt", "123", "12/25", "123", CurrencyType.USD);

        var cartItems = new List<CartItem>
        {
            new()
            {
                Id = 1,
                Quantity = 2
            }
        };

        var products = new List<Product>
        {
            new(1, "A", "B", "D", 50m, null, 10,
                DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "C")
        };

        var balance = new Balance(1000m);

        _cartApiProviderMock
            .Setup(x => x.GetCartItemsByUserId(command.Jwt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItems);

        _productApiProviderMock
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<string>(), It.IsAny<GetProductsByIdsDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        _paymentApiProviderMock
            .Setup(x => x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CurrencyType>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        await Assert.ThrowsAsync<PaymentFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
