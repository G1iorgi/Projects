using SharedKernel.CQRS;
using Shopping.Application.Aggregates.CartAggregate.Responses;

namespace Shopping.Application.Aggregates.CartAggregate.Queries.GetAllItems;

public record GetAllCartItemsQuery(string UserId,
    int PageSize,
    int PageNumber,
    string? ProductName = null,
    string? ProductDescription = null,
    decimal? PriceFrom = null,
    decimal? PriceTo = null,
    bool? HasImage = null) : IQuery<IEnumerable<ProductResponse>>;
