using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class TransactionNotFoundException(Guid transactionId) : NotFoundException(
    $"Transaction with ID {transactionId} not found.",
    "TRANSACTION_NOT_FOUND");
