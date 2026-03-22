using FluentValidation;

namespace Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.TotalPrice)
            .GreaterThan(0)
            .WithMessage("Total price must be greater than 0.");

        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid OrderStatus enum value.");

        RuleForEach(x => x.OrderItems)
            .NotEmpty()
            .NotNull()
            .WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new CreateOrderItemCommandValidator());
    }
}
