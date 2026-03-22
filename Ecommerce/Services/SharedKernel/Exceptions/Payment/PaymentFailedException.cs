using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Payment;

public sealed class PaymentFailedException() : BadGatewayException(
    "The payment process has failed.",
    "PAYMENT_FAILED");
