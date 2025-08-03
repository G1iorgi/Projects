using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Aggregates.IdentityAggregate.Configurations;

public sealed record TokenValidationOptions
{
    public const string Key = nameof(TokenValidationOptions);

    [Required]
    public bool ValidateIssuer { get; set; }

    [Required]
    public bool ValidateAudience { get; set; }

    [Required]
    public bool ValidateLifetime { get; set; }

    [Required]
    public bool ValidateIssuerSigningKey { get; set; }

    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    public required string Secret { get; set; }
}
