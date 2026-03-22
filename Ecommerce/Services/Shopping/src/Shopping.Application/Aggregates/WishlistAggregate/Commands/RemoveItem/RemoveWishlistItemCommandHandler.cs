using Ardalis.GuardClauses;
using MediatR;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Wishlist;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveItem;

public class RemoveWishlistItemCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<RemoveWishlistItemCommand>
{
    public async Task<Unit> Handle(RemoveWishlistItemCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var wishlist = await unitOfWork.Wishlists.GetByUserIdAsync(command.UserId, cancellationToken);
        if (wishlist is null)
            throw new WishlistNotFoundException(command.UserId);

        wishlist.RemoveItem(command.ProductId);

        unitOfWork.Wishlists.Update(wishlist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
