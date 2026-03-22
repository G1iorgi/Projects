using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class RefundFailedException() : BadGatewayException(
    "Refund transaction failed.",
    "REFUND_FAILED");
