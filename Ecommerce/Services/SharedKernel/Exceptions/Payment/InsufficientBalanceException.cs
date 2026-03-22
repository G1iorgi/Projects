using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class InsufficientBalanceException() : ConflictException(
    "Not enough balance.",
    "INSUFFICIENT_BALANCE");
