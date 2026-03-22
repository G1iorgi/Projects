using Ardalis.GuardClauses;
using Shopping.Application.Aggregates.WishlistAggregate.Queries.GetAllItems;

namespace Shopping.Presentation.Requests.WishlistRequest;

public record GetAllWishlistItemsRequest
{
    public required int PageSize { get; init; } = 10;

    public required int PageNumber { get; init; } = 1;

    public string? ProductName { get; init; }

    public string? ProductDescription { get; init; }

    public decimal? PriceFrom { get; init; }

    public decimal? PriceTo { get; init; }

    public bool? HasImage { get; init; }

    public GetAllWishlistItemsQuery ToQuery(string userId)
    {
        Guard.Against.NullOrWhiteSpace(userId);
        Guard.Against.NegativeOrZero(PageSize);
        Guard.Against.NegativeOrZero(PageNumber);

        return new GetAllWishlistItemsQuery(
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
