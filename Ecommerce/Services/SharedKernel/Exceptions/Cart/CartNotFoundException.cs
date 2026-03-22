using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Cart;

public sealed class CartNotFoundException(string userId) : NotFoundException(
    $"Cart for user with ID {userId} was not found.",
    "CART_NOT_FOUND");
