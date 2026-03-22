using SharedKernel.CQRS;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveAllItems;

public record RemoveAllWishlistItemsCommand(string UserId) : ICommand;
