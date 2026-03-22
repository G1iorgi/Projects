using FluentValidation;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Pay;

public sealed class PayCommandValidator : AbstractValidator<PayCommand>
{
    public PayCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.CreditCardNumber)
            .NotEmpty()
            .WithMessage("Credit card number is required.")
            .CreditCard()
            .WithMessage("Invalid credit card number.");

        RuleFor(x => x.ExpirationDate)
            .NotEmpty()
            .WithMessage("Expiration date is required.")
            .Matches(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$")
            .WithMessage("Expiration date must be in MM/YY format.");

        RuleFor(x => x.CVV)
            .NotEmpty()
            .WithMessage("CVV is required.")
            .Matches(@"^\d{3}$")
            .WithMessage("CVV must be 3 digits.");

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Currency must be a valid enum value.");
    }
}
