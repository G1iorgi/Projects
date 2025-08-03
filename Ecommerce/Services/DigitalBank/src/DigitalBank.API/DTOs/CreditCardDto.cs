namespace DigitalBank.API.DTOs;

public record CreditCardDto(
    string Number,
    string ExpirationDate,
    string CVV,
    List<BalanceDto> Balances);
