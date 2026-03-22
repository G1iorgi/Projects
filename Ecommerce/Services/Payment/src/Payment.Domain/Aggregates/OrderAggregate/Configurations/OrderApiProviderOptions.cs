using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Aggregates.OrderAggregate.Configurations;

public class OrderApiProviderOptions
{
    public const string Key = "OrderApiProviderOptions";

    [Required]
    public required string BaseUrl { get; set; }
}
