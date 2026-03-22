using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.CartAggregate.Queries.GetItemsByUserId;

namespace Shopping.Presentation.Requests.CartRequest;

public record GetItemsByUserIdRequest
{
 public GetCartItemsByUserIdQuery ToQuery(string userId)
 {
     Guard.Against.NullOrWhiteSpace(userId);

     return new GetCartItemsByUserIdQuery(UserId: userId);
 }
}
