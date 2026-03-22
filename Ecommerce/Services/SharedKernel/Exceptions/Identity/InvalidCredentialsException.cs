using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class InvalidCredentialsException() : UnauthorizedException(
    "Invalid username or password",
    "INVALID_CREDENTIALS");
