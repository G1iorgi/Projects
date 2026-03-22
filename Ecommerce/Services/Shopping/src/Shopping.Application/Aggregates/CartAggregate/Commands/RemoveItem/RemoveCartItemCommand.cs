using SharedKernel.CQRS;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.RemoveItem;

public record RemoveCartItemCommand(int ProductId, string UserId) : ICommand;
