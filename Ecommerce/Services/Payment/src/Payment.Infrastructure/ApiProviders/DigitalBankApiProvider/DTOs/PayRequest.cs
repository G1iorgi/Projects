using Payment.Domain.Aggregates.PaymentAggregate;

namespace Payment.Infrastructure.ApiProviders.DigitalBankApiProvider.DTOs;

internal record PayRequest(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency,
    decimal Amount);
