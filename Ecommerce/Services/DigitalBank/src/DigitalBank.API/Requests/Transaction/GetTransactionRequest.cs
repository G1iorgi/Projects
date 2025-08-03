using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.API.Requests.Transaction;

public record GetTransactionRequest([FromRoute] Guid TransactionId);

public class GetTransactionRequestValidator : AbstractValidator<GetTransactionRequest>
{
    public GetTransactionRequestValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}
