using System.Net.Http.Headers;
using System.Net.Http.Json;
using SharedKernel.Exceptions;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider;
using Shopping.Domain.Aggregates.ProductAggregate.ProductApiProvider.DTOs;

namespace Shopping.Infrastructure.ApiProviders.ProductApiProvider;

public class ProductApiProvider(HttpClient httpClient) : IProductApiProvider
{
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
}
