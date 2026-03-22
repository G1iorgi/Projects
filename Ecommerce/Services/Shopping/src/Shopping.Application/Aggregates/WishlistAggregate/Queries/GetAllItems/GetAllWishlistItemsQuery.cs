using SharedKernel.CQRS;
using Shopping.Application.Aggregates.WishlistAggregate.Responses;

namespace Shopping.Application.Aggregates.WishlistAggregate.Queries.GetAllItems;

public record GetAllWishlistItemsQuery(string UserId,
    int PageSize,
    int PageNumber,
    string? ProductName = null,
    string? ProductDescription = null,
    decimal? PriceFrom = null,
    decimal? PriceTo = null,
    bool? HasImage = null) : IQuery<IEnumerable<ProductResponse>>;
