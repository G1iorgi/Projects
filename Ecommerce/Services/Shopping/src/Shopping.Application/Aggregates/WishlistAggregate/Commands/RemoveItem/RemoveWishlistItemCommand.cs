using SharedKernel.CQRS;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveItem;

public record RemoveWishlistItemCommand(int ProductId, string UserId) : ICommand;
