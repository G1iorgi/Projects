using System.Security.Claims;
using Core.Application.Aggregates.WishlistAggregate;
using Core.Presentation.Request.WishlistRequest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Endpoints;

public class WishlistEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/wishlists")
            .WithTags("Wishlists")
            .RequireAuthorization();

        group.MapPost("/", AddProductAsync);
        group.MapGet("/", GetAllProductAsync);
        group.MapDelete("/", RemoveProductAsync);
        group.MapDelete("/all", RemoveAllProductsAsync);
    }

    private static async Task<IResult> AddProductAsync(
        WishlistService wishlistService,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] AddProductRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Results.Unauthorized();

        var command = AddProductRequest.ToCommand(request, userId);
        await wishlistService.AddProductAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetAllProductAsync(
        WishlistService wishlistService,
        IHttpContextAccessor httpContextAccessor,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] string? description = null,
        [FromQuery] decimal? priceFrom = null,
        [FromQuery] decimal? priceTo = null,
        [FromQuery] bool? hasImage = null,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Results.Unauthorized();

        var products = await wishlistService.GetAllProductAsync(
            userId,
            pageSize,
            pageNumber,
            name,
            description,
            priceFrom,
            priceTo,
            hasImage,
            cancellationToken);

        return Results.Ok(products);
    }

    private static async Task<IResult> RemoveProductAsync(
        WishlistService wishlistService,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] RemoveProductRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Results.Unauthorized();

        var command = RemoveProductRequest.ToCommand(request, userId);
        await wishlistService.RemoveProductAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> RemoveAllProductsAsync(
        WishlistService wishlistService,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        await wishlistService.RemoveAllProductsAsync(userId, cancellationToken);

        return Results.Ok();
    }
}
