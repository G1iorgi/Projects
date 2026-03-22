using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using SharedKernel.CQRS;

namespace SharedKernel.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.Null(next);

        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(e => e.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures is { Count: > 0 })
            throw new ValidationException(failures);

        return await next();
    }
}
