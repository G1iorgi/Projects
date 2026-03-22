using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Aggregates.CartAggregate.Configurations;

public class CartApiProviderOptions
{
    public const string Key = "CartApiProviderOptions";

    [Required]
    public required string BaseUrl { get; set; }
}
