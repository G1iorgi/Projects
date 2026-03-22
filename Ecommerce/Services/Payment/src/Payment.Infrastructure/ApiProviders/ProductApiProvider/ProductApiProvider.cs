using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ardalis.GuardClauses;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Payment.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;
using SharedKernel.Exceptions;

namespace Payment.Infrastructure.ApiProviders.ProductApiProvider;

public class ProductApiProvider(HttpClient httpClient) : IProductApiProvider
{
    public async Task<List<Product>?> GetProductsByIdsAsync(string jwt,
        GetProductsByIdsDto dto,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/products/productIds");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        request.Content = JsonContent.Create(dto);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<List<Product>>(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(string jwt,
        int productId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/products/{productId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<Product>(cancellationToken);
    }

    public async Task DecreaseProductsQuantityAsync(string jwt,
        DecreaseProductQuantitiesDto dto,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(dto);

        using var request = new HttpRequestMessage(HttpMethod.Put, "api/products/decrease");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        request.Content = JsonContent.Create(dto);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();
    }

    public async Task IncreaseProductsQuantityAsync(string jwt,
        IncreaseProductQuantitiesDto dto,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(dto);

        using var request = new HttpRequestMessage(HttpMethod.Put, "api/products/increase");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        request.Content = JsonContent.Create(dto);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();
    }
}
