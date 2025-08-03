using DigitalBank.API.Enums;
using FluentValidation;

namespace DigitalBank.API.Requests.CreditCard;

public record GetBalanceRequest(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency) :
    BaseCreditCardData(CreditCardNumber, ExpirationDate, CVV);

public class GetBalanceRequestValidator : AbstractValidator<GetBalanceRequest>
{
    public GetBalanceRequestValidator()
    {
        Include(new BaseCreditCardDataValidator());

        RuleFor(x => x.Currency).IsInEnum();
    }
}
