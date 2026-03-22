using Microsoft.AspNetCore.Http;

namespace SharedKernel.Exceptions.HttpResponseExceptions;

// Represents an error when the client request is invalid or fails validation rules.
public class ValidationException(string message, string? errorCode = null)
    : BaseException(message, StatusCodes.Status400BadRequest, errorCode);
