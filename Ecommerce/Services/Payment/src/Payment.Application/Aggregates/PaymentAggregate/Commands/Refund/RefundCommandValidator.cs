using FluentValidation;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Refund;

public sealed class RefundCommandValidator : AbstractValidator<RefundCommand>
{
    public RefundCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0.");
    }
}
