using Ardalis.GuardClauses;
using MediatR;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Cart;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.RemoveAllItems;

public class RemoveAllCartItemsCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<RemoveAllCartItemsCommand>
{
    public async Task<Unit> Handle(RemoveAllCartItemsCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var cart = await unitOfWork.Carts.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart is null)
            throw new CartNotFoundException(request.UserId);

        foreach (var item in cart.CartItems.ToList())
        {
            cart.RemoveItem(item.ProductId);
        }

        unitOfWork.Carts.Update(cart);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
