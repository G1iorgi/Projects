using System.Text.Json;
using Ardalis.GuardClauses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace SharedKernel.Middlewares.GlobalExceptionHandlingMiddleware;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Guard.Against.Null(context);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        logger.LogError(exception, "Unhandled exception ({ExceptionType}) TraceId: {TraceId}",
            exception.GetType().Name, traceId);

        var response = exception switch
        {
            BaseException baseException => new ApiErrorResponse(
                baseException.StatusCode,
                baseException.Message,
                traceId,
                baseException.ErrorCode),

            ValidationException validationException => new ApiErrorResponse(
                StatusCodes.Status400BadRequest,
                validationException.Message,
                traceId,
                validationException.GetType().Name),

            _ => new ApiErrorResponse(
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                traceId,
                "UNHANDLED_EXCEPTION")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
