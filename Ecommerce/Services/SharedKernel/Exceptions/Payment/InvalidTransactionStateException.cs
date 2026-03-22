using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class InvalidTransactionStateException(Guid transactionId) : ConflictException(
    $"Transaction with ID {transactionId} is not in a valid state for refund.",
    "INVALID_TRANSACTION_STATE");
