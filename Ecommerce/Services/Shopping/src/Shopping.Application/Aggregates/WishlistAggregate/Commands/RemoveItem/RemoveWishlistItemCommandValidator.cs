using FluentValidation;

namespace Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveItem;

public sealed class RemoveWishlistItemCommandValidator : AbstractValidator<RemoveWishlistItemCommand>
{
    public RemoveWishlistItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than 0.");
    }
}
