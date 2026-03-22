using SharedKernel.CQRS;
using Shopping.Application.Aggregates.CartAggregate.Responses;

namespace Shopping.Application.Aggregates.CartAggregate.Queries.GetItemsByUserId;

public record GetCartItemsByUserIdQuery(string UserId) : IQuery<List<ProductResponse>>;
