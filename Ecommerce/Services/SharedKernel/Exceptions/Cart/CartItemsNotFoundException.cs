using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Cart;

public sealed class CartItemsNotFoundException : NotFoundException
{
    public CartItemsNotFoundException()
        : base("Some products not found.",
            "CART_ITEMS_NOT_FOUND")
    {
    }

    public CartItemsNotFoundException(IEnumerable<int> missingIds)
        : base($"Products with IDs {string.Join(", ", missingIds)} not found.",
            "CART_ITEMS_NOT_FOUND")
    {
    }
}
