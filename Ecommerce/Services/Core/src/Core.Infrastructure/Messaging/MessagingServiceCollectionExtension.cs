using Ardalis.GuardClauses;
using Core.Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel.Configurations;

namespace Core.Infrastructure.Messaging;

internal static class MessagingServiceCollectionExtension
{
    internal static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerOptions>(
            configuration.GetSection(MessageBrokerOptions.Key));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<ProductsQuantitiesIncreasedConsumer>();
            busConfigurator.AddConsumer<ProductsQuantitiesDecreasedConsumer>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var messageBrokerOptions = context.GetRequiredService<IOptions<MessageBrokerOptions>>().Value;

                Guard.Against.NullOrWhiteSpace(messageBrokerOptions.Host);
                Guard.Against.NullOrWhiteSpace(messageBrokerOptions.UserName);
                Guard.Against.NullOrWhiteSpace(messageBrokerOptions.Password);
                Guard.Against.NullOrWhiteSpace(messageBrokerOptions.QueueName);

                cfg.Host(new Uri(messageBrokerOptions.Host), h =>
                {
                    h.Username(messageBrokerOptions.UserName);
                    h.Password(messageBrokerOptions.Password);
                });
                cfg.ReceiveEndpoint(messageBrokerOptions.QueueName, e =>
                {
                    e.ConfigureConsumer<ProductsQuantitiesIncreasedConsumer>(context);
                    e.ConfigureConsumer<ProductsQuantitiesDecreasedConsumer>(context);
                });
            });
        });
        return services;
    }
}
