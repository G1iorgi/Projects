using Ardalis.GuardClauses;
using Dapper;
using SharedKernel.Contracts.Abstractions.Data;
using SharedKernel.CQRS;
using Shopping.Application.Aggregates.WishlistAggregate.Responses;

namespace Shopping.Application.Aggregates.WishlistAggregate.Queries.GetAllItems;

public class GetAllWishlistItemsQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetAllWishlistItemsQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(
        GetAllWishlistItemsQuery query,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);

        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                wi."ProductId" AS Id,
                wi."ProductName" AS Name,
                wi."ProductDescription" AS Description,
                wi."ProductPrice" AS Price,
                wi."ProductImage" AS Image
            FROM "Shopping"."Wishlist" w
            INNER JOIN "Shopping"."WishlistItem" wi
                ON wi."WishlistId" = w."Id"
            WHERE w."UserId" = @UserId

            AND (
                @ProductName IS NULL
                OR wi."ProductName" ILIKE '%' || @ProductName || '%'
            )

            AND (
                @ProductDescription IS NULL
                OR wi."ProductDescription" ILIKE '%' || @ProductDescription || '%'
            )

            AND (
                @PriceFrom IS NULL
                OR wi."ProductPrice" >= @PriceFrom
            )

            AND (
                @PriceTo IS NULL
                OR wi."ProductPrice" <= @PriceTo
            )

            AND (
                @HasImage IS NULL
                OR (
                    @HasImage = true
                    AND wi."ProductImage" IS NOT NULL
                    AND wi."ProductImage" <> ''
                )
                OR (
                    @HasImage = false
                    AND (
                        wi."ProductImage" IS NULL
                        OR wi."ProductImage" = ''
                    )
                )
            )

            ORDER BY wi."ProductName"
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
