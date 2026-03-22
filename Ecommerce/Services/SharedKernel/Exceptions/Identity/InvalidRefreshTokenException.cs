using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class InvalidRefreshTokenException() : UnauthorizedException(
    "Invalid refresh token.",
    "INVALID_REFRESH_TOKEN");
