using Ardalis.GuardClauses;
using SharedKernel.CQRS;
using SharedKernel.Exceptions.Cart;
using Shopping.Application.Aggregates.CartAggregate.Responses;
using Shopping.Domain;

namespace Shopping.Application.Aggregates.CartAggregate.Queries.GetItemsByUserId;

public class GetCartItemsByUserIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetCartItemsByUserIdQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetCartItemsByUserIdQuery query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query);

        var cart = await unitOfWork.Carts.GetByUserIdAsync(query.UserId, cancellationToken);

        if (cart is null)
            throw new EmptyCartException();

        return cart.CartItems
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
    }
}
