namespace Core.Application.Aggregates.UserAggregate.Commands;

public record RegisterUserCommand
{
    public required string Username { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}
