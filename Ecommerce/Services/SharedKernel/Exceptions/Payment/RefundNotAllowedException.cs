using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class RefundNotAllowedException() : ConflictException(
    "Refund is only allowed within 14 days of the transaction",
    "REFUND_NOT_ALLOWED");
