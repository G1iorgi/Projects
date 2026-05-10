using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Payment.Domain;
using SharedKernel.Configurations;
using SharedKernel.Contracts.Abstractions;

namespace Payment.Infrastructure.Messaging;

internal static class MessagingServiceCollectionExtensions
{
    internal static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerOptions>(
            configuration.GetSection(MessageBrokerOptions.Key));

        services.AddScoped<IEventBus, MassTransitEventBus>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;

                Guard.Against.NullOrWhiteSpace(options.Host);
                Guard.Against.NullOrWhiteSpace(options.UserName);
                Guard.Against.NullOrWhiteSpace(options.Password);

                cfg.Host(new Uri(options.Host), h =>
                {
                    h.Username(options.UserName);
                    h.Password(options.Password);
                });
            });
        });

        return services;
    }
}
