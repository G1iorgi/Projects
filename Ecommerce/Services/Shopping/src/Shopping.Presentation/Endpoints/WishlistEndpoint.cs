using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shopping.Presentation.Requests.WishlistRequest;

namespace Shopping.Presentation.Endpoints;

public class WishlistEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/wishlists")
            .WithTags("Wishlists")
            .RequireAuthorization();

        group.MapPost("/", AddItemAsync);
        group.MapGet("/", GetAllItemsAsync);
        group.MapDelete("/", RemoveItemAsync);
        group.MapDelete("/all", RemoveAllItemsAsync);
    }

    private static async Task<IResult> AddItemAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] AddWishlistItemRequest? request,
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

        var command = AddWishlistItemRequest.ToCommand(request, userId, jwt);
        await mediator.Send(command, cancellationToken);

        return Results.Created();
    }

    private static async Task<IResult> GetAllItemsAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [AsParameters] GetAllWishlistItemsRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var query = request.ToQuery(userId);
        var items = await mediator.Send(query, cancellationToken);

        return Results.Ok(items);
    }

    private static async Task<IResult> RemoveItemAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] RemoveWishlistItemRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var command = RemoveWishlistItemRequest.ToCommand(request, userId);
        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> RemoveAllItemsAsync(IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var command = RemoveAllWishlistItemsRequest.ToCommand(userId);

        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
