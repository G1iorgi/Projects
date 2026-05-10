using System.Net.Http.Headers;
using System.Net.Http.Json;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider;
using Payment.Domain.Aggregates.OrderAggregate.OrderApiProvider.DTOs;
using SharedKernel.Exceptions;

namespace Payment.Infrastructure.ApiProviders.OrderApiProvider;

public class OrderApiProvider(HttpClient httpClient) : IOrderApiProvider
{
    public async Task<Order?> GetOrderByIdAsync(string jwt,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/orders/{orderId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<Order>(cancellationToken);
    }
}
