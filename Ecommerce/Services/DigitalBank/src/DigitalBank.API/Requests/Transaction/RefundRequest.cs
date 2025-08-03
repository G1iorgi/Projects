using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.API.Requests.Transaction;

public record RefundRequest([FromRoute] Guid TransactionId);

public class RefundRequestValidator : AbstractValidator<RefundRequest>
{
    public RefundRequestValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}
