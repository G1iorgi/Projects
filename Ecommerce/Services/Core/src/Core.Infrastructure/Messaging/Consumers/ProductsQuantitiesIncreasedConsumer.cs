using Ardalis.GuardClauses;
using Core.Application.Aggregates.ProductAggregate;
using Core.Application.Aggregates.ProductAggregate.Commands;
using Core.Application.Aggregates.ProductAggregate.DTOs;
using MassTransit;
using SharedKernel.Contracts.Events;

namespace Core.Infrastructure.Messaging.Consumers;

internal sealed class ProductsQuantitiesIncreasedConsumer(ProductService productService)
    : IConsumer<ProductsQuantitiesIncreasedEvent>
{
    public async Task Consume(ConsumeContext<ProductsQuantitiesIncreasedEvent> context)
    {
        Guard.Against.Null(context);

        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var command = new IncreaseProductsQuantityCommand
        {
            Items = message.Items
                .Select(x => new ProductQuantityDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })
                .ToList()
        };

        await productService.IncreaseProductsQuantityAsync(command, cancellationToken);
    }
}
