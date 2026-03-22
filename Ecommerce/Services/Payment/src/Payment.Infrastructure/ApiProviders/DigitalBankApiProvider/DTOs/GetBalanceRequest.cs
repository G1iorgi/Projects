using Payment.Domain.Aggregates.PaymentAggregate;

namespace Payment.Infrastructure.ApiProviders.DigitalBankApiProvider.DTOs;

internal record GetBalanceRequest(
    string CreditCardNumber,
    string ExpirationDate,
    string CVV,
    CurrencyType Currency);
