using Ardalis.GuardClauses;
using MediatR;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider.DTOs;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Cart;
using SharedKernel.Exceptions.Payment;
using SharedKernel.Exceptions.Product;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.PayFromCart;

public class PayFromCartCommandHandler(
    IPaymentApiProvider paymentApiProvider,
    ICartApiProvider cartApiProvider,
    IProductApiProvider productApiProvider,
    IOrderApiProvider orderApiProvider) : ICommandHandler<PayFromCartCommand>
{
    public async Task<Unit> Handle(PayFromCartCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        // TODO : Uncomment when User Management is ready
        // _ = await userManager.FindByIdAsync(command.UserId) ??
        //     throw new UserNotFoundException()
        var cartItems = await cartApiProvider.GetCartItemsByUserId(command.Jwt, cancellationToken);

        if (cartItems is null or { Count: <= 0 })
            throw new EmptyCartException();

        var productIds = cartItems.Select(p => p.Id).ToList();
        var getProductsByIdsDto = new GetProductsByIdsDto
        {
            ProductIds = productIds
        };

        var products = await productApiProvider.GetProductsByIdsAsync(command.Jwt,
            getProductsByIdsDto,
            cancellationToken);

        ValidateAllProductsExist(products, cartItems);

        decimal totalPrice = 0;
        var orderItems = new List<CreateOrderItemDTO>();
        foreach (var item in cartItems)
        {
            var product = products!.Find(p => p.Id == item.Id)
                          ?? throw new ProductNotFoundException(item.Id);

            if (product.Quantity < item.Quantity)
                throw new InsufficientProductQuantityException(item.Id);

            totalPrice += item.Quantity * product.Price;

            orderItems.Add(new CreateOrderItemDTO
            {
                ProductId = product.Id,
                Price = product.Price,
                Quantity = item.Quantity,
            });
        }

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

        var order = new CreateOrderDTO
        {
            UserId = command.UserId,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            Status = OrderStatus.Completed,
            OrderItems = orderItems,
        };

        await orderApiProvider.CreateOrderAsync(command.Jwt, order, cancellationToken);

        var dto = new DecreaseProductQuantitiesDto
        {
            Items = cartItems
                .Select(item => new ProductQuantityDto(item.Id, item.Quantity))
                .ToList()
        };

        await productApiProvider.DecreaseProductsQuantityAsync(command.Jwt, dto, cancellationToken);

        await cartApiProvider.RemoveAllItemsByUserId(command.Jwt, cancellationToken);

        return Unit.Value;
    }

    private static void ValidateAllProductsExist(List<Product> products,
        List<CartItem> cartItems)
    {
        Guard.Against.Null(products);
        Guard.Against.Null(cartItems);

        if (products.Count != cartItems.Count)
            throw new CartItemsNotFoundException();

        var foundIds = products.Select(p => p.Id).ToHashSet();
        var missingIds = cartItems
            .Select(p => p.Id)
            .Where(id => !foundIds.Contains(id))
            .ToList();

        if (missingIds is { Count: > 0 })
            throw new CartItemsNotFoundException(missingIds);
    }

    private static void ValidateBalance(Balance? balanceResponse, decimal totalPrice)
    {
        if (balanceResponse is null)
            throw new BalanceNotFoundException();

        if (balanceResponse.Amount < totalPrice)
            throw new InsufficientBalanceException();
    }
}
