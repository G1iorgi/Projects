using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate.Commands;

namespace Core.Presentation.Request.ProductRequest;

public record GetProductsByIdsRequest
{
    public required IReadOnlyList<int> ProductIds { get; init; } = new List<int>();

    public static GetProductsByIdsCommand ToCommand(GetProductsByIdsRequest request)
    {
        Guard.Against.Null(request);

        return new GetProductsByIdsCommand
        {
            ProductIds = request.ProductIds
        };
    }
}
