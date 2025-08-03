using DigitalBank.API.Enums;
using FluentValidation;

namespace DigitalBank.API.Requests.CreditCard;

public record BaseTransactionData(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency,
    decimal Amount) :
    BaseCreditCardData(CreditCardNumber, ExpirationDate, CVV);

public class BaseTransactionDataValidator : AbstractValidator<BaseTransactionData>
{
    public BaseTransactionDataValidator()
    {
        Include(new BaseCreditCardDataValidator());

        RuleFor(x => x.CreditCardNumber)
            .NotEmpty()
            .CreditCard();

        RuleFor(x => x.Currency)
            .IsInEnum();

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
