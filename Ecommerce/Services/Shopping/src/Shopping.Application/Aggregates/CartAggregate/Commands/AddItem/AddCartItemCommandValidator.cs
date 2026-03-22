using FluentValidation;

namespace Shopping.Application.Aggregates.CartAggregate.Commands.AddItem;

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than 0.");

        RuleFor(x => x.ProductQuantity)
            .GreaterThan(0)
            .WithMessage("ProductQuantity must be greater than 0.");
    }
}
