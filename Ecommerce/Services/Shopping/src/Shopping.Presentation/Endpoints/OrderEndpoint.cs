using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shopping.Presentation.Requests.OrderRequest;

namespace Shopping.Presentation.Endpoints;

public class OrderEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/orders")
            .WithTags("Orders")
            .RequireAuthorization();

        group.MapGet("/{orderId:int}", GetOrderByIdAsync);
    }

    private static async Task<IResult> GetOrderByIdAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [AsParameters] GetOrderByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var query = GetOrderByIdRequest.ToQuery(request);

        var order = await mediator.Send(query, cancellationToken);

        return Results.Ok(order);
    }
}
