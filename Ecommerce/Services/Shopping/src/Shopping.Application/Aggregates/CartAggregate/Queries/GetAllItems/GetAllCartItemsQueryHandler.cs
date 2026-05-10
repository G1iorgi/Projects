using Ardalis.GuardClauses;
using Dapper;
using SharedKernel.Contracts.Abstractions.Data;
using SharedKernel.CQRS;
using Shopping.Application.Aggregates.CartAggregate.Responses;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.CartAggregate.Queries.GetAllItems;

public class GetAllCartItemsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllCartItemsQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllCartItemsQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);

        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
                           SELECT
                               ci."ProductId" AS Id,
                               ci."ProductName" AS Name,
                               ci."ProductDescription" AS Description,
                               ci."ProductPrice" AS Price,
                               ci."ProductQuantity" AS Quantity,
                               ci."ProductImage" AS Image
                           FROM "Shopping"."Cart" c
                           INNER JOIN "Shopping"."CartItem" ci
                               ON ci."CartId" = c."Id"
                           WHERE c."UserId" = @UserId
                               AND (
                                   @ProductName IS NULL
                                   OR ci."ProductName" ILike '%' || @ProductName || '%'
                               )
                               AND (
                                   @ProductDescription IS NULL
                                   OR ci."ProductDescription" ILIKE '%' || @ProductDescription || '%'
                               )
                               AND (
                                   @PriceFrom IS NULL
                                   OR ci."ProductPrice" >= @PriceFrom
                               )
                               AND (
                                   @PriceTo IS NULL
                                   OR ci."ProductPrice" <= @PriceTo
                               )
                               AND (
                                   @HasImage IS NULL
                                   OR (
                                       @HasImage = true
                                       AND ci."ProductImage" IS NOT NULL
                                       AND ci."ProductImage" <> ''
                                   )
                                   OR (
                                       @HasImage = false
                                       AND (
                                           ci."ProductImage" IS NULL
                                           OR ci."ProductImage" = ''
                                       )
                                   )
                               )
                           ORDER BY ci."ProductImage"
                           OFFSET @Offset
                           LIMIT @PageSize;
                           """;

        var items = await connection.QueryAsync<ProductResponse>(
            sql,
            new
            {
                query.UserId,
                query.ProductName,
                query.ProductDescription,
                query.PriceFrom,
                query.PriceTo,
                query.HasImage,
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            });

        return items;
    }
}
