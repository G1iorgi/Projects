using Microsoft.AspNetCore.Http;

namespace SharedKernel.Exceptions.HttpResponseExceptions;

public class NotFoundException(string message, string? errorCode = null)
    : BaseException(message, StatusCodes.Status404NotFound, errorCode);
