using SharedKernel.CQRS;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.RemoveAllItems;

public record RemoveAllCartItemsCommand(string UserId) : ICommand;
