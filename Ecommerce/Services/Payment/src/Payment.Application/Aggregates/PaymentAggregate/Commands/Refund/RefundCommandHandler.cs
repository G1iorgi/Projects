using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Options;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;
using Payment.Domain.Aggregates.PaymentAggregate.Configurations;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using SharedKernel.Contracts.Abstractions;
using SharedKernel.Contracts.Events;
using SharedKernel.Contracts.Events.DTOs;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Order;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Refund;

public class RefundCommandHandler(
    IPaymentApiProvider paymentApiProvider,
    IProductApiProvider productApiProvider,
    IOrderApiProvider orderApiProvider,
    IEventBus eventBus,
    IOptions<DigitalBankOptions> options) : ICommandHandler<RefundCommand>
{
    private readonly DigitalBankOptions _digitalBankOptions = options.Value;

    public async Task<Unit> Handle(RefundCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        // TODO : Uncomment when User Management is ready
        // _ = await userManager.FindByIdAsync(command.UserId) ??
        //     throw new UserNotFoundException();
        var originalOrder = await orderApiProvider.GetOrderByIdAsync(command.Jwt, command.OrderId, cancellationToken) ??
                            throw new OrderNotFoundException(command.OrderId);

        var previousTransaction =
            await paymentApiProvider.GetTransaction(originalOrder.TransactionId, cancellationToken);
        ValidatePreviousTransaction(previousTransaction, originalOrder);

        var refundedTransactionId =
            await paymentApiProvider.RefundAsync(originalOrder.TransactionId, cancellationToken);

        if (refundedTransactionId == Guid.Empty)
            throw new RefundFailedException();

        var orderRefundedEvent = new OrderRefundedEvent(
            command.UserId,
            DateTimeOffset.UtcNow,
            originalOrder.TotalPrice,
            refundedTransactionId,
            OrderStatuses.Refunded,
            originalOrder.OrderItems.Select(item => new OrderItemDTO(
                item.ProductId,
                item.Quantity,
                item.Price)).ToList());
        await eventBus.PublishAsync(orderRefundedEvent, cancellationToken);

        var productIds = originalOrder.OrderItems.Select(oi => oi.ProductId).ToList();
        var getProductsByIdsDto = new GetProductsByIdsDto
        {
            ProductIds = productIds,
        };
        var products =
            await productApiProvider.GetProductsByIdsAsync(command.Jwt, getProductsByIdsDto, cancellationToken);

        var items = originalOrder.OrderItems
            .Select(item =>
            {
                var product = products!.Find(p => p.Id == item.ProductId)
                              ?? throw new ProductNotFoundException(item.ProductId);

                return new ProductQuantityDTO(product.Id, item.Quantity);
            })
            .ToList();

        var productsQuantityIncreasedEvent = new ProductsQuantitiesIncreasedEvent(items);
        await eventBus.PublishAsync(productsQuantityIncreasedEvent, cancellationToken);

        return Unit.Value;
    }

    private void ValidatePreviousTransaction(Transaction? previousTransaction,
        Order originalOrder)
    {
        if (previousTransaction is null)
            throw new TransactionNotFoundException(originalOrder.TransactionId);

        if (previousTransaction.Status != TransactionStatus.Completed)
            throw new InvalidTransactionStateException(originalOrder.TransactionId);

        if (previousTransaction.CreateDate < DateTimeOffset.Now.AddDays(_digitalBankOptions.RefundTimeLimitDays * -1))
            throw new RefundNotAllowedException();
    }
}
