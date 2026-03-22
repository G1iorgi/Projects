using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class BalanceNotFoundException() : NotFoundException(
    "Balance response is null.",
    "BALANCE_NOT_FOUND");
