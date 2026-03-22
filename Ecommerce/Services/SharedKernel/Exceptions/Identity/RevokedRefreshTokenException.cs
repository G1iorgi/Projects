using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class RevokedRefreshTokenException() : UnauthorizedException(
    "The refresh token has been revoked.",
    "REVOKED_REFRESH_TOKEN");
