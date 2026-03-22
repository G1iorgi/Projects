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
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Order;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Refund;

public class RefundCommandHandler(
    IOrderApiProvider orderApiProvider,
    IPaymentApiProvider paymentApiProvider,
    IProductApiProvider productApiProvider,
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
        if (originalOrder is null)
        {
            throw new OrderNotFoundException(command.OrderId);
        }

        var previousTransaction =
            await paymentApiProvider.GetTransaction(originalOrder.TransactionId, cancellationToken);
        ValidatePreviousTransaction(previousTransaction, originalOrder);

        var refundedTransactionId =
            await paymentApiProvider.RefundAsync(originalOrder.TransactionId, cancellationToken);

        if (refundedTransactionId == Guid.Empty)
            throw new RefundFailedException();

        var refundedOrder = new CreateOrderDTO
        {
            UserId = command.UserId,
            TotalPrice = originalOrder.TotalPrice,
            TransactionId = refundedTransactionId,
            Status = OrderStatus.Refunded,
            OrderItems = originalOrder.OrderItems
                .Select(item => new CreateOrderItemDTO
                {
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                })
                .ToList(),
        };

        await orderApiProvider.CreateOrderAsync(command.Jwt, refundedOrder, cancellationToken);

        var productIds = originalOrder.OrderItems.Select(oi => oi.ProductId).ToList();
        var getProductsByIdsDto = new GetProductsByIdsDto
        {
            ProductIds = productIds,
        };
        var products =
            await productApiProvider.GetProductsByIdsAsync(command.Jwt, getProductsByIdsDto, cancellationToken);

        var dto = new IncreaseProductQuantitiesDto
        {
            Items = originalOrder.OrderItems
                .Select(item =>
                {
                    var product = products!.Find(p => p.Id == item.ProductId)
                                  ?? throw new ProductNotFoundException(item.ProductId);
                    return new ProductQuantityDto(product.Id, item.Quantity);
                })
                .ToList(),
        };

        await productApiProvider.IncreaseProductsQuantityAsync(command.Jwt, dto, cancellationToken);

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
