using System.ComponentModel.DataAnnotations;

namespace Payment.Presentation.Configurations;

public sealed record TokenValidationOptions
{
    public const string Key = "TokenValidationOptions";

    [Required]
    public bool ValidateIssuer { get; set; }

    [Required]
    public bool ValidateAudience { get; set; }

    [Required]
    public bool ValidateLifetime { get; set; }

    [Required]
    public bool ValidateIssuerSigningKey { get; set; }

    [Required]
    public string Issuer { get; set; }

    [Required]
    public string Audience { get; set; }

    [Required]
    public string Secret { get; set; }
}
