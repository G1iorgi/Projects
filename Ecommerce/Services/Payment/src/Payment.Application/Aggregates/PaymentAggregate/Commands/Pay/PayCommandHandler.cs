using Ardalis.GuardClauses;
using MediatR;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using SharedKernel.Contracts.Abstractions;
using SharedKernel.Contracts.Events;
using SharedKernel.Contracts.Events.DTOs;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Pay;

public class PayCommandHandler(
    IProductApiProvider productApiProvider,
    IPaymentApiProvider paymentApiProvider,
    IEventBus eventBus) : ICommandHandler<PayCommand>
{
    public async Task<Unit> Handle(PayCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        // TODO : Uncomment when User Management is ready
        // _ = await userManager.FindByIdAsync(command.UserId) ??
        var product = await productApiProvider.GetProductByIdAsync(command.Jwt, command.ProductId, cancellationToken) ??
                      throw new ProductNotFoundException(command.ProductId);

        if (product.Quantity < command.Quantity)
            throw new InsufficientProductQuantityException(command.ProductId);

        var totalPrice = product.Price * command.Quantity;

        var balance = await paymentApiProvider.GetBalance(command.CreditCardNumber,
            command.ExpirationDate,
            command.CVV,
            command.Currency,
            cancellationToken);

        ValidateBalance(balance, totalPrice);

        var transactionId = await paymentApiProvider.PayAsync(command.CreditCardNumber,
            command.ExpirationDate,
            command.CVV,
            command.Currency,
            totalPrice,
            cancellationToken);

        if (transactionId == Guid.Empty)
            throw new PaymentFailedException();

        var orderItems = new List<OrderItemDTO>
        {
            new(product.Id, command.Quantity, product.Price)
        };
        var orderCreatedEvent = new OrderCreatedEvent(
            command.UserId,
            DateTimeOffset.UtcNow,
            totalPrice,
            transactionId,
            OrderStatuses.Completed,
            orderItems);
        await eventBus.PublishAsync(orderCreatedEvent, cancellationToken);

        var item = orderItems
            .Select(item => new ProductQuantityDTO(item.ProductId, item.Quantity))
            .ToList();

        var productsQuantityDecreasedEvent = new ProductsQuantitiesDecreasedEvent(item);
        await eventBus.PublishAsync(productsQuantityDecreasedEvent, cancellationToken);

        return Unit.Value;
    }

    private static void ValidateBalance(Balance? balanceResponse, decimal totalPrice)
    {
        if (balanceResponse is null)
            throw new BalanceNotFoundException();

        if (balanceResponse.Amount < totalPrice)
            throw new InsufficientBalanceException();
    }
}
