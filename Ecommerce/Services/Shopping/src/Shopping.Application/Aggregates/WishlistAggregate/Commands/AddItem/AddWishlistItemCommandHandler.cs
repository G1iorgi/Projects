using Ardalis.GuardClauses;
using MediatR;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Product;
using Shopping.Domain;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.AddItem;

public class AddWishlistItemCommandHandler(IUnitOfWork unitOfWork,
    IProductApiProvider productApiProvider) : ICommandHandler<AddWishlistItemCommand>
{
    public async Task<Unit> Handle(AddWishlistItemCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var product = await productApiProvider.GetProductByIdAsync(command.Jwt, command.ProductId, cancellationToken);
        ValidateProduct(product, command);

        var wishlist = await unitOfWork.Wishlists.GetByUserIdAsync(command.UserId, cancellationToken);
        wishlist ??= Wishlist.Create(command.UserId);
        wishlist.AddItem(command.ProductId, product!.Name, product.Price);

        unitOfWork.Wishlists.Update(wishlist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static void ValidateProduct(Product? product, AddWishlistItemCommand command)
    {
        if (product is null)
            throw new ProductNotFoundException(command.ProductId);
    }
}
