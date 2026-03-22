using System.Net.Http.Json;
using Payment.Domain.Aggregates.PaymentAggregate;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider;
using Payment.Domain.Aggregates.PaymentAggregate.PaymentApiProvider.DTOs;
using Payment.Infrastructure.ApiProviders.DigitalBankApiProvider.DTOs;
using SharedKernel.Exceptions;

namespace Payment.Infrastructure.ApiProviders.DigitalBankApiProvider;

public class DigitalBankApiProvider(HttpClient httpClient) : IPaymentApiProvider
{
    public async Task<Guid> PayAsync(string creditCardNumber,
        string expirationDate,
        string cvv,
        CurrencyType currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var request = new PayRequest(creditCardNumber,
            expirationDate,
            cvv,
            currency,
            amount);

        var response = await httpClient.PostAsJsonAsync("api/credit-cards/pay", request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);
    }

    public async Task<Balance?> GetBalance(string creditCardNumber,
        string expirationDate,
        string cvv,
        CurrencyType currency,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/credit-cards/get-balance");
        request.Content = JsonContent.Create(new GetBalanceRequest(creditCardNumber,
            expirationDate,
            cvv,
            currency));

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<Balance>(cancellationToken);
    }

    public async Task<Transaction?> GetTransaction(Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/transactions/{transactionId}");
        request.Content = JsonContent.Create(new GetTransactionRequest(transactionId));

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<Transaction>(cancellationToken);
    }

    public async Task<TransactionStatus> GetTransactionStatus(Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/transactions/{transactionId}/status");
        request.Content = JsonContent.Create(new GetTransactionStatusRequest(transactionId));

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<TransactionStatus>(cancellationToken);
    }

    public async Task<Guid> RefundAsync(Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/transactions/{transactionId}/refund");
        request.Content = JsonContent.Create(new RefundRequest(transactionId));

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            throw new ApiProviderException();

        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);
    }
}

