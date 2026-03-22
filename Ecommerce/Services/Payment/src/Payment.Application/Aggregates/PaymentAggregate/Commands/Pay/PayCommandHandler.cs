using Ardalis.GuardClauses;
using MediatR;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Pay;

public class PayCommandHandler(
    IProductApiProvider productApiProvider,
    IPaymentApiProvider paymentApiProvider,
    IOrderApiProvider orderApiProvider) : ICommandHandler<PayCommand>
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

        var createOrderDto = new CreateOrderDTO
        {
            UserId = command.UserId,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            Status = OrderStatus.Completed,
            OrderItems =
            [
                new CreateOrderItemDTO
                {
                    ProductId = product.Id,
                    Price = product.Price,
                    Quantity = command.Quantity,
                }
            ]
        };

        await orderApiProvider.CreateOrderAsync(command.Jwt, createOrderDto, cancellationToken);

        var dto = new DecreaseProductQuantitiesDto
        {
            Items =
            [
                new ProductQuantityDto(product.Id, command.Quantity)
            ],
        };

        await productApiProvider.DecreaseProductsQuantityAsync(command.Jwt, dto, cancellationToken);

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
