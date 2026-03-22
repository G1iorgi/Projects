namespace SharedKernel.Middlewares.GlobalExceptionHandlingMiddleware;

public record ApiErrorResponse(
    int StatusCode,
    string Message,
    string TraceId,
    string? ErrorCode = null);
