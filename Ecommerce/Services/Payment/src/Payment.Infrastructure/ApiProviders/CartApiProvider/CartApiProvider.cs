using System.Net.Http.Headers;
using System.Net.Http.Json;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider;
using Payment.Domain.Aggregates.CartAggregate.CartApiProvider.DTOs;
using SharedKernel.Exceptions;

namespace Payment.Infrastructure.ApiProviders.CartApiProvider;

public class CartApiProvider(HttpClient httpClient) : ICartApiProvider
{
    public async Task<List<CartItem>?> GetCartItemsByUserId(string jwt,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/carts/by-user-id");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        var items = await response.Content.ReadFromJsonAsync<List<CartItem>>(cancellationToken);
        return items ?? [];
    }

    public async Task RemoveAllItemsByUserId(string jwt, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, "api/carts/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();
    }
}
