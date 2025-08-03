using Core.Application.Aggregates.CartAggregate;
using Core.Application.Aggregates.CategoryAggregate;
using Core.Application.Aggregates.ProductAggregate;
using Core.Application.Aggregates.UserAggregate;
using Core.Application.Aggregates.WishlistAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Aggregates;

internal static class ServicesServiceCollectionExtensions
{
    internal static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<UserService>();
        services.AddScoped<WishlistService>();
        services.AddScoped<CartService>();
    }
}
