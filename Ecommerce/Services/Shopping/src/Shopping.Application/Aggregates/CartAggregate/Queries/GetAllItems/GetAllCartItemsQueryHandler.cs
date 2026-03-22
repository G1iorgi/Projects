using Ardalis.GuardClauses;
using SharedKernel.CQRS;
using Shopping.Application.Aggregates.CartAggregate.Responses;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.CartAggregate.Queries.GetAllItems;

public class GetAllCartItemsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllCartItemsQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllCartItemsQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);

        var cart = await unitOfWork.Carts.GetByUserIdAsync(query.UserId, cancellationToken);
        var items = cart?.CartItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.ProductName))
            items = items?.Where(i => i.ProductName.Contains(query.ProductName));

        if (!string.IsNullOrWhiteSpace(query.ProductDescription))
            items = items?.Where(i => i.ProductDescription != null && i.ProductDescription.Contains(query.ProductDescription));

        if (query.PriceFrom.HasValue)
            items = items?.Where(i => i.ProductPrice >= query.PriceFrom.Value);

        if (query.PriceFrom.HasValue)
            items = items?.Where(i => i.ProductPrice <= query.PriceTo!.Value);

        items = query.HasImage.HasValue
            ? items?.Where(i => query.HasImage.Value
                ? !string.IsNullOrWhiteSpace(i.ProductImage)
                : string.IsNullOrWhiteSpace(i.ProductImage))
            : items;

        var pagedItems = items?
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductResponse
            {
                Id = p.ProductId,
                Name = p.ProductName,
                Description = p.ProductDescription,
                Price = p.ProductPrice,
                Quantity = p.ProductQuantity,
                Image = p.ProductImage
            })
            .ToList();

        return pagedItems!;
    }
}
