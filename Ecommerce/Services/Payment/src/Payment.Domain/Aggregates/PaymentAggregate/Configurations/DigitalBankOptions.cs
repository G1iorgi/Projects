using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Aggregates.PaymentAggregate.Configurations;

public class DigitalBankOptions
{
    public const string Key = "PaymentProviderOptions";

    [Required]
    public required string BaseUrl { get; set; }

    [Range(1, 365)]
    public required int RefundTimeLimitDays { get; set; }
}
