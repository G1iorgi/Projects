using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Cart;

public sealed class EmptyCartException() : ValidationException(
    "The cart is empty.",
    "EMPTY_CART");
