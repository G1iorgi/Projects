using Ardalis.GuardClauses;
using SharedKernel.Pagination;
using Shopping.Application.Aggregates.CartAggregate.Queries.GetAllItems;

namespace Shopping.Presentation.Requests.CartRequest;

public record GetAllCartItemsRequest : PaginatedRequest
{
    public string? ProductName { get; init; }

    public string? ProductDescription { get; init; }

    public decimal? PriceFrom { get; init; }

    public decimal? PriceTo { get; init; }

    public bool? HasImage { get; init; }

    public GetAllCartItemsQuery ToQuery(string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        Guard.Against.NegativeOrZero(PageSize);
        Guard.Against.NegativeOrZero(PageNumber);

        return new GetAllCartItemsQuery(
            UserId: userId,
            PageSize: PageSize,
            PageNumber: PageNumber,
            ProductName: ProductName,
            ProductDescription: ProductDescription,
            PriceFrom: PriceFrom,
            PriceTo: PriceTo,
            HasImage: HasImage);
    }
}
