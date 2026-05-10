using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel.Configurations;
using Shopping.Infrastructure.Messaging.Consumers;

namespace Shopping.Infrastructure.Messaging;

internal static class MessagingServiceCollectionExtensions
{
    internal static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerOptions>(
            configuration.GetSection(MessageBrokerOptions.Key));

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<OrderCreatedConsumer>();
            busConfigurator.AddConsumer<OrderRefundedConsumer>();
            busConfigurator.AddConsumer<CartEmptiedConsumer>();

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
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    e.ConfigureConsumer<OrderRefundedConsumer>(context);
                    e.ConfigureConsumer<CartEmptiedConsumer>(context);
                });
            });
        });

        return services;
    }
}
