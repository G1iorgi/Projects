using Ardalis.GuardClauses;
using MediatR;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Cart;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.RemoveItem;

public class RemoveCartItemCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<RemoveCartItemCommand>
{
    public async Task<Unit> Handle(RemoveCartItemCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command);

        var cart = await unitOfWork.Carts.GetByUserIdAsync(command.UserId, cancellationToken);
        if (cart is null)
            throw new CartNotFoundException(command.UserId);

        cart.RemoveItem(command.ProductId);

        unitOfWork.Carts.Update(cart);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
