using FluentValidation;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.PayFromCart;

public sealed class PayFromCartCommandValidator : AbstractValidator<PayFromCartCommand>
{
    public PayFromCartCommandValidator()
    {
        RuleFor(x => x.CreditCardNumber)
            .NotEmpty()
            .WithMessage("Credit card number is required.")
            .Length(16)
            .WithMessage("Credit card number must be 16 digits.")
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
