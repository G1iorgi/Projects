using SharedKernel.CQRS;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.AddItem;

public record AddWishlistItemCommand(string UserId,
    string Jwt,
    int ProductId) : ICommand;
