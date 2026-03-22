using Ardalis.GuardClauses;
using MediatR;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Product;
using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.AddItem;

public class AddCartItemCommandHandler(IUnitOfWork unitOfWork,
    IProductApiProvider productApiProvider) : ICommandHandler<AddCartItemCommand>
{
    public async Task<Unit> Handle(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var product = await productApiProvider.GetProductByIdAsync(command.Jwt, command.ProductId, cancellationToken);
        ValidateProduct(product, command);

        var cart = await unitOfWork.Carts.GetByUserIdAsync(command.UserId, cancellationToken);
        cart ??= Cart.Create(command.UserId);
        cart.AddItem(command.ProductId, product!.Name, command.ProductQuantity, product.Price);

        unitOfWork.Carts.Update(cart);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static void ValidateProduct(Product? product, AddCartItemCommand command)
    {
        if (product is null)
            throw new ProductNotFoundException(command.ProductId);

        if (command.ProductQuantity > product.Quantity)
            throw new InsufficientProductQuantityException(command.ProductId);
    }
}
