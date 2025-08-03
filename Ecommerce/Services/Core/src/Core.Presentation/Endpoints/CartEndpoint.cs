using System.Security.Claims;
using Core.Application.Aggregates.CartAggregate;
using Core.Presentation.Request.CartRequest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Endpoints;

public class CartEndpoint : IEndpoint
{
    // TODO need to implement a buy product endpoint
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/carts")
            .WithTags("Carts")
            .RequireAuthorization();

        group.MapPost("/", AddProductAsync);
        group.MapGet("/", GetAllProductAsync);
        group.MapDelete("/", RemoveProductAsync);
        group.MapDelete("/all", RemoveAllProductsAsync);
    }

    private static async Task<IResult> AddProductAsync(
        CartService cartService,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] AddProductToCartRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Results.Unauthorized();

        var command = AddProductToCartRequest.ToCommand(request, userId);
        await cartService.AddProductAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetAllProductAsync(
        CartService cartService,
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

        var products = await cartService.GetAllProductAsync(
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
        CartService cartService,
        IHttpContextAccessor httpContextAccessor,
        [FromBody] RemoveProductFromCartRequest? request,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Results.Unauthorized();

        var command = RemoveProductFromCartRequest.ToCommand(request, userId);
        await cartService.RemoveProductAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> RemoveAllProductsAsync(
        CartService cartService,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

        await cartService.RemoveAllProductsAsync(userId, cancellationToken);

        return Results.Ok();
    }
}
