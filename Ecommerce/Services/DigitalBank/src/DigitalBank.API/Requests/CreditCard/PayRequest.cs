using DigitalBank.API.Enums;
using FluentValidation;

namespace DigitalBank.API.Requests.CreditCard;

public record PayRequest(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency,
    decimal Amount) :
    BaseTransactionData(CreditCardNumber, ExpirationDate, CVV, Currency, Amount);

public class PayRequestValidator : AbstractValidator<PayRequest>
{
    public PayRequestValidator()
    {
        Include(new BaseTransactionDataValidator());
    }
}
