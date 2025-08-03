using Core.Application.Aggregates.CategoryAggregate;
using Core.Presentation.Request.CategoryRequest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Endpoints;

public class CategoryEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder
            .MapGroup("api/categories")
            .WithTags("Categories")
            .RequireAuthorization();

        group.MapGet("/", GetCategories);
        group.MapPost("/", CreateCategory);
        group.MapGet("/{categoryId:int}", GetCategoryById);
        group.MapPut("/{categoryId:int}", UpdateCategory);
        group.MapDelete("/{categoryId:int}", DeleteCategory);
    }

    private static async Task<IResult> GetCategories(
        CategoryService categoryService,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] int? productQuantityFrom = null,
        [FromQuery] int? productQuantityTo = null,
        CancellationToken cancellationToken = default)
    {
        var categories = await categoryService.GetAllCategoriesAsync(
            pageSize,
            pageNumber,
            name,
            productQuantityFrom,
            productQuantityTo,
            cancellationToken);

        return Results.Ok(categories);
    }

    private static async Task<IResult> CreateCategory(
        CategoryService categoryService,
        [FromBody] CreateCategoryRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = CreateCategoryRequest.ToCommand(request);
        await categoryService.CreateCategoryAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetCategoryById(
        CategoryService categoryService,
        [FromRoute] int categoryId,
        CancellationToken cancellationToken = default)
    {
        var result = await categoryService.GetCategoryByIdAsync(categoryId, cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateCategory(
        CategoryService categoryService,
        [FromBody] UpdateCategoryRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = UpdateCategoryRequest.ToCommand(request);
        await categoryService.UpdateCategoryAsync(command, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> DeleteCategory(
        CategoryService categoryService,
        [FromRoute] int categoryId,
        CancellationToken cancellation = default)
    {
        await categoryService.DeleteCategoryAsync(categoryId, cancellation);

        return Results.Ok();
    }
}
