using Ardalis.GuardClauses;
using MassTransit;
using MediatR;
using SharedKernel.Contracts.Events;
using Shopping.Application.Aggregates.OrderAggregate.Commands.CreateOrder;
using OrderStatus = Shopping.Domain.Aggregates.OrderAggregate.OrderStatus;

namespace Shopping.Infrastructure.Messaging.Consumers;

internal sealed class OrderCreatedConsumer(IMediator mediator) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Guard.Against.Null(context);

        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var command = new CreateOrderCommand(
            message.UserId,
            message.TotalPrice,
            message.TransactionId,
            OrderStatus.Completed,
            message.OrderItems
                .Select(oi => new CreateOrderItemCommand(
                    oi.ProductId,
                    oi.Price,
                    oi.Quantity))
                .ToList());

        await mediator.Send(command, cancellationToken);
    }
}
