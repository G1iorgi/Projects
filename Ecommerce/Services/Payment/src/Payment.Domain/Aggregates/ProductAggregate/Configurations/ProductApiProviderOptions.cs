using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Aggregates.ProductAggregate.Configurations;

public class ProductApiProviderOptions
{
    public const string Key = "ProductApiProviderOptions";

    [Required]
    public required string BaseUrl { get; set; }
}
