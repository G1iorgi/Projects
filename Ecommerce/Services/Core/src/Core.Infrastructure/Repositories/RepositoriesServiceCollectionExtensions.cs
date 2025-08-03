using Core.Domain;
using Core.Domain.Aggregates.CartAggregate;
using Core.Domain.Aggregates.CategoryAggregate;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.ProductAggregate;
using Core.Domain.Aggregates.WishlistAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Repositories;

internal static class RepositoriesServiceCollectionExtensions
{
    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
