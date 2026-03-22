using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class UserAlreadyExistsException(string userName) : ValidationException(
    $"User with username '{userName}' already exists.",
    "USER_ALREADY_EXISTS");
