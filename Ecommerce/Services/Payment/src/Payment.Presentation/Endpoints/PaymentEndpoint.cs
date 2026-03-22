using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Payment.Presentation.Requests;

namespace Payment.Presentation.Endpoints;

public class PaymentEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/payments")
            .WithTags("Payments")
            .RequireAuthorization();

        group.MapPost("/cart/pay", PayFromCartAsync);
        group.MapPost("/pay", PayAsync);
        group.MapPost("{orderId}/refund", RefundAsync);
    }

    private static async Task<IResult> PayFromCartAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] PayFromCartRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var jwt = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString()
            .Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrWhiteSpace(jwt))
            return Results.Unauthorized();

        var command = PayFromCartRequest.ToCommand(request, userId, jwt);

        await mediator.Send(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> PayAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] PayRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var jwt = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString()
            .Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrWhiteSpace(jwt))
            return Results.Unauthorized();

        var command = PayRequest.ToCommand(request, userId, jwt);

        await mediator.Send(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> RefundAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] RefundRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var jwt = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString()
            .Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrWhiteSpace(jwt))
            return Results.Unauthorized();

        var command = RefundRequest.ToCommand(request, userId, jwt);

        await mediator.Send(command, cancellationToken);

        return Results.Ok();
    }
}
