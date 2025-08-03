using Ardalis.GuardClauses;
using Core.Application.Aggregates.WishlistAggregate.Commands;
using Core.Application.Aggregates.WishlistAggregate.Responses;
using Core.Domain;
using Core.Domain.Aggregates.WishlistAggregate;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Aggregates.WishlistAggregate;

public class WishlistService(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
{
    public async Task AddProductAsync(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        _ = await userManager.FindByIdAsync(command.UserId) ??
            throw new ArgumentException($"User with ID {command.UserId} doesn't exist");

        _ = await unitOfWork.Products.GetByIdAsync(command.ProductId, cancellationToken) ??
            throw new ArgumentException($"Product with ID {command.ProductId} doesn't exist");

        var wishlistItem = await unitOfWork.Wishlists.GetWishlistItemAsync(
            command.UserId,
            command.ProductId,
            cancellationToken);
        if (wishlistItem != null)
            throw new ArgumentException("Product is already in wishlist");

        var wishlistItemToAdd = Wishlist.Create(command.UserId, command.ProductId);
        await unitOfWork.Wishlists.CreateAsync(wishlistItemToAdd, cancellationToken);
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
        var wishlists = unitOfWork.Wishlists.GetAll()
            .Where(w => w.UserId == userId);

        if (!string.IsNullOrWhiteSpace(name))
            wishlists = wishlists.Where(w => w.Product.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(description))
        {
            wishlists = wishlists.Where(
                w =>
                    w.Product.Description != null &&
                    w.Product.Description.Contains(description));
        }

        if (priceFrom.HasValue)
            wishlists = wishlists.Where(w => w.Product.Price >= priceFrom.Value);

        if (priceTo.HasValue)
            wishlists = wishlists.Where(w => w.Product.Price <= priceTo.Value);

        wishlists = hasImage switch
        {
            true => wishlists.Where(w => !string.IsNullOrWhiteSpace(w.Product.Image)),
            false => wishlists.Where(w => string.IsNullOrWhiteSpace(w.Product.Image)),
            _ => wishlists
        };

        return await Task.FromResult(
            wishlists
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => w.Product)
                .Select(
                    p => new ProductResponse
                    {
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Image = p.Image,
                    })
                .ToList());
    }

    public async Task RemoveProductAsync(RemoveProductCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        _ = await userManager.FindByIdAsync(command.UserId) ??
            throw new ArgumentException($"User with ID {command.UserId} doesn't exist");

        _ = await unitOfWork.Products.GetByIdAsync(command.ProductId, cancellationToken) ??
            throw new ArgumentException($"Product with ID {command.ProductId} does not exist");

        var wishlistItem = await unitOfWork.Wishlists.GetWishlistItemAsync(
            command.UserId,
            command.ProductId,
            cancellationToken) ?? throw new ArgumentException("Product is not in wishlist");

        await unitOfWork.Wishlists.DeleteAsync(wishlistItem.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAllProductsAsync(string userId, CancellationToken cancellationToken = default)
    {
        _ = await userManager.FindByIdAsync(userId) ??
            throw new ArgumentException($"User with ID {userId} doesn't exist");

        var wishlistItems = await unitOfWork.Wishlists
            .GetWishlistItemsAsync(userId, cancellationToken);

        foreach (var item in wishlistItems)
        {
            await unitOfWork.Wishlists.DeleteAsync(item.Id, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
