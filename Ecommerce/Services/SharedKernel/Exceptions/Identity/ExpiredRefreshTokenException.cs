using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class ExpiredRefreshTokenException() : UnauthorizedException(
    "The refresh token has expired.",
    "EXPIRED_REFRESH_TOKEN");
