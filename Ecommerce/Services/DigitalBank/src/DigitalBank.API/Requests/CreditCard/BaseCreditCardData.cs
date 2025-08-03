using FluentValidation;

namespace DigitalBank.API.Requests.CreditCard;

public record BaseCreditCardData(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV);

public class BaseCreditCardDataValidator : AbstractValidator<BaseCreditCardData>
{
    public BaseCreditCardDataValidator()
    {
        RuleFor(x => x.CreditCardNumber)
            .NotEmpty()
            .CreditCard();

        RuleFor(x => x.ExpirationDate)
            .NotEmpty()
            .Length(5)
            .Matches(@"^\d{2}/\d{2}$")
            .WithMessage("Expiration Day And Month is invalid")
            .Must(BeAValidDate).WithMessage("expiration Date is invalid");

        RuleFor(x => x.CVV)
            .NotEmpty()
            .Length(3)
            .Matches(@"^\d{3}$")
            .WithMessage("CVV is invalid");
    }

    private bool BeAValidDate(string expirationDate)
    {
        var parts = expirationDate.Split('/');

        if (!int.TryParse(parts[0], out var month) || !int.TryParse(parts[1], out var year)) return false;

        if (month is <= 0 or > 12) return false;

        var expirationDateTime = new DateTime(year + 2000, month, 1).AddMonths(1).AddDays(-1);
        return expirationDateTime > DateTime.UtcNow;
    }
}
