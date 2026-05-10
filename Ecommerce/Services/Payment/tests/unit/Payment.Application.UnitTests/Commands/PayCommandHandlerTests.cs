using Moq;
using Payment.Application.Aggregates.PaymentAggregate.Commands.Pay;
using Payment.Domain.Aggregates.PaymentAggregate;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using SharedKernel.Contracts.Abstractions;
using SharedKernel.Contracts.Events;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.UnitTests.Commands;

public class PayCommandHandlerTests
{
    private readonly Mock<IProductApiProvider> _productApiProviderMock = new();
    private readonly Mock<IPaymentApiProvider> _paymentApiProviderMock = new();
    private readonly Mock<IEventBus> _eventBusMock = new();

    private readonly PayCommandHandler _handler;

    public PayCommandHandlerTests()
    {
        _handler = new PayCommandHandler(
            _productApiProviderMock.Object,
            _paymentApiProviderMock.Object,
            _eventBusMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPublishEventsAndSucceed()
    {
        var command = new PayCommand("user-1", "jwt", 1, 2,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        var balance = new Balance(1000m);
        var transactionId = Guid.NewGuid();
        var expectedTotal = product.Price * command.Quantity;

        _productApiProviderMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        _paymentApiProviderMock
            .Setup(x => x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), expectedTotal, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);

        await _handler.Handle(command, CancellationToken.None);

        _paymentApiProviderMock.Verify(x =>
            x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), expectedTotal, It.IsAny<CancellationToken>()),
            Times.Once);

        _eventBusMock.Verify(x =>
            x.PublishAsync(It.Is<OrderCreatedEvent>(e =>
                e.UserId == command.UserId &&
                e.TotalPrice == expectedTotal &&
                e.TransactionId == transactionId &&
                e.OrderItems.Single().ProductId == command.ProductId &&
                e.OrderItems.Single().Quantity == command.Quantity),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _eventBusMock.Verify(x =>
            x.PublishAsync(It.Is<ProductsQuantitiesDecreasedEvent>(e =>
                e.Items.Single().ProductId == command.ProductId &&
                e.Items.Single().Quantity == command.Quantity),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ShouldThrow()
    {
        var command = new PayCommand("user-1", "jwt", 999, 1,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        _productApiProviderMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _eventBusMock.Verify(x =>
                x.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _eventBusMock.Verify(x =>
                x.PublishAsync(It.IsAny<ProductsQuantitiesDecreasedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInsufficientQuantity_ShouldThrow()
    {
        var command = new PayCommand("user-1", "jwt", 1, 100,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        _productApiProviderMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        await Assert.ThrowsAsync<InsufficientProductQuantityException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenBalanceNotFound_ShouldThrow()
    {
        var command = new PayCommand("user-1", "jwt", 1, 2,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        _productApiProviderMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Balance?)null);

        await Assert.ThrowsAsync<BalanceNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenInsufficientBalance_ShouldThrow()
    {
        var command = new PayCommand("user-1", "jwt", 1, 10,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        var balance = new Balance(100m); // not enough

        _productApiProviderMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        await Assert.ThrowsAsync<InsufficientBalanceException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPaymentFails_ShouldThrow()
    {
        var command = new PayCommand("user-1", "jwt", 1, 2,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        var balance = new Balance(1000m);

        _productApiProviderMock
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApiProviderMock
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        _paymentApiProviderMock
            .Setup(x => x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        await Assert.ThrowsAsync<PaymentFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _eventBusMock.Verify(x =>
                x.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _eventBusMock.Verify(x =>
                x.PublishAsync(It.IsAny<ProductsQuantitiesDecreasedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
