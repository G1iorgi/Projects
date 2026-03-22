using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class UserNotFoundException() : NotFoundException(
    "User not found.",
    "USER_NOT_FOUND");
