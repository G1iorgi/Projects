using Ardalis.GuardClauses;
using Core.Application.Aggregates.CartAggregate.Commands;
using Core.Application.Aggregates.CartAggregate.Responses;
using Core.Domain;
using Core.Domain.Aggregates.CartAggregate;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Aggregates.CartAggregate;

public class CartService(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
{
    public async Task AddProductAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        _ = await userManager.FindByIdAsync(command.UserId) ??
            throw new ArgumentException($"User with ID {command.UserId} doesn't exist");

        _ = await unitOfWork.Products.GetByIdAsync(command.ProductId, cancellationToken) ??
            throw new ArgumentException($"Product with ID {command.ProductId} doesn't exist");

        var cartItem = await unitOfWork.Carts.GetCartItemAsync(
            command.UserId,
            command.ProductId,
            cancellationToken);
        if (cartItem != null)
            throw new ArgumentException("Product is already in cart");

        var cartItemToAdd = Cart.Create(command.UserId, command.ProductId);
        await unitOfWork.Carts.CreateAsync(cartItemToAdd, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllProductAsync(
        string userId,
        int pageSize,
        int pageNumber,
        string? name = null,
        string? description = null,
        decimal? priceFrom = null,
        decimal? priceTo = null,
        bool? hasImage = null,
        CancellationToken cancellationToken = default)
    {
        var carts = unitOfWork.Carts.GetAll()
            .Where(w => w.UserId == userId);

        if (!string.IsNullOrWhiteSpace(name))
            carts = carts.Where(w => w.Product.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(description))
        {
            carts = carts.Where(
                w =>
                    w.Product.Description != null &&
                    w.Product.Description.Contains(description));
        }

        if (priceFrom.HasValue)
            carts = carts.Where(w => w.Product.Price >= priceFrom.Value);

        if (priceTo.HasValue)
            carts = carts.Where(w => w.Product.Price <= priceTo.Value);

        carts = hasImage switch
        {
            true => carts.Where(w => !string.IsNullOrWhiteSpace(w.Product.Image)),
            false => carts.Where(w => string.IsNullOrWhiteSpace(w.Product.Image)),
            _ => carts
        };

        return await Task.FromResult(
            carts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => w.Product)
                .Select(
                    p => new ProductResponse
                    {
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Image = p.Image
                    })
                .ToList());
    }

    public async Task RemoveProductAsync(RemoveProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        _ = await userManager.FindByIdAsync(command.UserId) ??
            throw new ArgumentException($"User with ID {command.UserId} doesn't exist");

        _ = await unitOfWork.Products.GetByIdAsync(command.ProductId, cancellationToken) ??
            throw new ArgumentException($"Product with ID {command.ProductId} doesn't exist");

        var cartItem = await unitOfWork.Carts.GetCartItemAsync(
            command.UserId,
            command.ProductId,
            cancellationToken) ?? throw new ArgumentException("Product is not in wishlist");

        await unitOfWork.Carts.DeleteAsync(cartItem.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAllProductsAsync(string userId, CancellationToken cancellationToken = default)
    {
        _ = userManager.FindByIdAsync(userId) ??
            throw new ArgumentException($"User with ID {userId} doesn't exist");

        var cartItems = await unitOfWork.Carts
            .GetCartItemsAsync(userId, cancellationToken);

        foreach (var item in cartItems)
        {
            await unitOfWork.Carts.DeleteAsync(item.Id, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
