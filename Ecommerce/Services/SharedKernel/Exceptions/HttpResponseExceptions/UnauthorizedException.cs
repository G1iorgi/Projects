using Microsoft.AspNetCore.Http;

namespace SharedKernel.Exceptions.HttpResponseExceptions;

public class UnauthorizedException(string message, string? errorCode = null)
    : BaseException(message, StatusCodes.Status401Unauthorized, errorCode);
