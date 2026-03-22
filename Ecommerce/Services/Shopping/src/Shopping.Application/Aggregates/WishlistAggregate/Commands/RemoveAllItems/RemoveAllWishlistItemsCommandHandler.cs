using Ardalis.GuardClauses;
using MediatR;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Wishlist;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveAllItems;

public class RemoveAllWishlistItemsCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveAllWishlistItemsCommand>
{
    public async Task<Unit> Handle(RemoveAllWishlistItemsCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var wishlist = await unitOfWork.Wishlists.GetByUserIdAsync(command.UserId, cancellationToken);
        if (wishlist is null)
            throw new WishlistNotFoundException(command.UserId);

        foreach (var item in wishlist.WishlistItems.ToList())
        {
            wishlist.RemoveItem(item.ProductId);
        }

        unitOfWork.Wishlists.Update(wishlist);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
