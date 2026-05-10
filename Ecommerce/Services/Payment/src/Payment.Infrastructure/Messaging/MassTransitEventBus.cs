using Ardalis.GuardClauses;
using MassTransit;
using SharedKernel.Contracts.Abstractions;

namespace Payment.Infrastructure;

internal sealed class MassTransitEventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        Guard.Against.Null(@event);

        await publishEndpoint.Publish(@event, cancellationToken);
    }
}
