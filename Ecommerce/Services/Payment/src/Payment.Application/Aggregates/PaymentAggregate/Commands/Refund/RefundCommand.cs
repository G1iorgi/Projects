using SharedKernel.CQRS;

namespace Payment.Application.Aggregates.PaymentAggregate.Commands.Refund;

public record RefundCommand(string UserId,
    string Jwt,
    int OrderId) : ICommand;
