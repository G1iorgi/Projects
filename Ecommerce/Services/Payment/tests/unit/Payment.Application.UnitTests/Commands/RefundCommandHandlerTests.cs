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

public class RefundCommandHandlerTests
{
    private readonly Mock<IProductApiProvider> _productApi = new();
    private readonly Mock<IPaymentApiProvider> _paymentApi = new();
    private readonly Mock<IEventBus> _eventBus = new();

    private readonly PayCommandHandler _handler;

    public RefundCommandHandlerTests()
    {
        _handler = new PayCommandHandler(
            _productApi.Object,
            _paymentApi.Object,
            _eventBus.Object);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndPublishEvents()
    {
        var command = new PayCommand("user-1", "jwt", 1, 2,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(
            1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        var balance = new Balance(1000m);
        var transactionId = Guid.NewGuid();

        _productApi
            .Setup(x => x.GetProductByIdAsync(command.Jwt, command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApi
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        _paymentApi
            .Setup(x => x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), product.Price * command.Quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);

        OrderCreatedEvent? orderEvent = null;
        ProductsQuantitiesDecreasedEvent? qtyEvent = null;

        _eventBus
            .Setup(x => x.PublishAsync(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<OrderCreatedEvent, CancellationToken>((e, _) => orderEvent = e)
            .Returns(Task.CompletedTask);

        _eventBus
            .Setup(x => x.PublishAsync(It.IsAny<ProductsQuantitiesDecreasedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<ProductsQuantitiesDecreasedEvent, CancellationToken>((e, _) => qtyEvent = e)
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(orderEvent);
        Assert.Equal(command.UserId, orderEvent.UserId);
        Assert.Equal(transactionId, orderEvent.TransactionId);

        Assert.NotNull(qtyEvent);
        Assert.Single(qtyEvent.Items);
        Assert.Equal(command.ProductId, qtyEvent.Items[0].ProductId);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenProductNotFound()
    {
        var command = new PayCommand("user-1", "jwt", 999, 1,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        _productApi
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _paymentApi.Verify(x => x.PayAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<CurrencyType>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenInsufficientBalance()
    {
        var command = new PayCommand("user-1", "jwt", 1, 10,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(
            1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        var balance = new Balance(100m);

        _productApi
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApi
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        await Assert.ThrowsAsync<InsufficientBalanceException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPaymentFails()
    {
        var command = new PayCommand("user-1", "jwt", 1, 2,
            "4532015112830366", "12/25", "123", CurrencyType.USD);

        var product = new Product(
            1, "Test", "B", "D", 50m, null, 10,
            DateTimeOffset.UtcNow, ProductStatus.Enabled, 1, "Cat");

        var balance = new Balance(1000m);

        _productApi
            .Setup(x => x.GetProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _paymentApi
            .Setup(x => x.GetBalance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        _paymentApi
            .Setup(x => x.PayAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CurrencyType>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

        await Assert.ThrowsAsync<PaymentFailedException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _eventBus.Verify(x => x.PublishAsync(
            It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
