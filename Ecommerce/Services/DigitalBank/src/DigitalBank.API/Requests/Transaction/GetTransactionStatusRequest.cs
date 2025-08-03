using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.API.Requests.Transaction;

public record GetTransactionStatusRequest([FromRoute] Guid TransactionId);

public class StatusRequestValidator : AbstractValidator<GetTransactionStatusRequest>
{
    public StatusRequestValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}
