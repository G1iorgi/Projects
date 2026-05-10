using Ardalis.GuardClauses;
using Dapper;
using SharedKernel.Contracts.Abstractions.Data;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Cart;
using Shopping.Application.Aggregates.CartAggregate.Responses;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.CartAggregate.Queries.GetItemsByUserId;

public class GetCartItemsByUserIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetCartItemsByUserIdQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetCartItemsByUserIdQuery query,
        CancellationToken cancellationToken)
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
                           """;

        var items = await connection.QueryAsync<ProductResponse>(
            sql,
            new { query.UserId });

        var productResponses = items.ToList();
        if (!productResponses.Any())
            throw new EmptyCartException();

        return productResponses.ToList();
    }
}
