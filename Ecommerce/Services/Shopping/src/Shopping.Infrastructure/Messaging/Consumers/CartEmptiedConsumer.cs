using Ardalis.GuardClauses;
using MassTransit;
using MediatR;
using SharedKernel.Contracts.Events;
using Shopping.Application.Aggregates.CartAggregate.Commands.RemoveAllItems;

namespace Shopping.Infrastructure.Messaging.Consumers;

internal sealed class CartEmptiedConsumer(IMediator mediator) : IConsumer<CartEmptiedEvent>
{
    public async Task Consume(ConsumeContext<CartEmptiedEvent> context)
    {
        Guard.Against.Null(context);

        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var command = new RemoveAllCartItemsCommand(message.UserId);

        await mediator.Send(command, cancellationToken);
    }
}
