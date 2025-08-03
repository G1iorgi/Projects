namespace Core.Application.Aggregates.UserAggregate.Commands;

public record LoginUserCommand
{
    public required string Username { get; init; }

    public required string Password { get; init; }
}
