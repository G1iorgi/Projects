using System.Text;
using Ardalis.GuardClauses;
using Core.Domain.Aggregates.IdentityAggregate.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Core.Presentation.ServiceCollectionExtensions;

internal static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddCoreAuthentication(this IServiceCollection services)
    {
        var jwtConfig = services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<TokenValidationOptions>>()
            .Value;

        Guard.Against.Null(jwtConfig);

        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtConfig.ValidateIssuer,
                    ValidateAudience = jwtConfig.ValidateAudience,
                    ValidateLifetime = jwtConfig.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtConfig.ValidateIssuerSigningKey,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
                };
            });

        return services;
    }
}
