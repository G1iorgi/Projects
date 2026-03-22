using Microsoft.Extensions.DependencyInjection;
using Shopping.Domain;
using Shopping.Domain.Aggregates.CartAggregate;
using Shopping.Domain.Aggregates.OrderAggregate;
using Shopping.Domain.Aggregates.WishlistAggregate;

namespace Shopping.Infrastructure.Repositories;

internal static class RepositoriesServiceCollectionExtensions
{
    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}
