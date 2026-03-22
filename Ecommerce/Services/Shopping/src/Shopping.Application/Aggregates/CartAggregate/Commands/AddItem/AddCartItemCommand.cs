using SharedKernel.CQRS;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.AddItem;

public record AddCartItemCommand(string UserId,
    string Jwt,
    int ProductId,
    int ProductQuantity) : ICommand;
