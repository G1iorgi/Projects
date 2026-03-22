using Microsoft.AspNetCore.Http;

namespace SharedKernel.Exceptions.HttpResponseExceptions;

// Represents a conflict with the current state of the system.
public class ConflictException(string message, string? errorCode = null)
    : BaseException(message, StatusCodes.Status409Conflict, errorCode);
