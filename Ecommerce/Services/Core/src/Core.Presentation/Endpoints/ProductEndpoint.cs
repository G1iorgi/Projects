using Core.Application.Aggregates.ProductAggregate;
using Core.Presentation.Request.ProductRequest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Endpoints;

public class ProductEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/products")
            .WithTags("Products")
            .RequireAuthorization();

        group.MapGet("/", GetProducts);
        group.MapPost("/", CreateProduct);
        group.MapGet("/{productId:int}", GetProductById);
        group.MapPut("/{productId:int}", UpdateProduct);
        group.MapDelete("/{productId:int}", DeleteProduct);
    }

    private static async Task<IResult> GetProducts(
        ProductService productService,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] string? barcode = null,
        [FromQuery] string? description = null,
        [FromQuery] decimal? priceFrom = null,
        [FromQuery] decimal? priceTo = null,
        [FromQuery] bool? hasImage = null,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllProductsAsync(
            pageSize,
            pageNumber,
            name,
            barcode,
            description,
            priceFrom,
            priceTo,
            hasImage,
            cancellationToken);

        return Results.Ok(products);
    }

    private static async Task<IResult> CreateProduct(
        ProductService productService,
        [FromBody] CreateProductRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = CreateProductRequest.ToCommand(request);
        await productService.CreateProductAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetProductById(
        ProductService productService,
        [FromRoute] int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await productService.GetProductByIdAsync(productId, cancellationToken);

        return Results.Ok(product);
    }

    private static async Task<IResult> UpdateProduct(
        ProductService productService,
        [FromBody] UpdateProductRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = UpdateProductRequest.ToCommand(request);
        await productService.UpdateProductAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> DeleteProduct(
        ProductService productService,
        [FromRoute] int productId,
        CancellationToken cancellationToken = default)
    {
        await productService.DeleteProductAsync(productId, cancellationToken);

        return Results.Ok();
    }
}
