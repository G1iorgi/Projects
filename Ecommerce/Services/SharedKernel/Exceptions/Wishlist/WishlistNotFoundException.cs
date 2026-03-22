using Microsoft.AspNetCore.Http;
using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Wishlist;

public sealed class WishlistNotFoundException(string userId) : NotFoundException(
    $"Wishlist for user with ID {userId} was not found.",
    "WISHLIST_NOT_FOUND");
