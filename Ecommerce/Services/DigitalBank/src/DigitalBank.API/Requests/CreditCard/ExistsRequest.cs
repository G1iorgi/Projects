using FluentValidation;

namespace DigitalBank.API.Requests.CreditCard;

public record ExistsRequest(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV) :
    BaseCreditCardData(CreditCardNumber, ExpirationDate, CVV);

public class ExistsRequestValidator : AbstractValidator<ExistsRequest>
{
    public ExistsRequestValidator()
    {
        Include(new BaseCreditCardDataValidator());
    }
}
