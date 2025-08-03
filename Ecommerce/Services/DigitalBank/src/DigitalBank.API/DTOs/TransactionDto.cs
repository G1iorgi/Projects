using DigitalBank.API.Enums;

namespace DigitalBank.API.DTOs;

public record TransactionDto(
    Guid Id,
    decimal Amount,
    string CreditCardNumber,
    TransactionStatus Status,
    DateTimeOffset CreateDate);
