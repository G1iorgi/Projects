namespace Core.Application.Aggregates.UserAggregate.Commands;

public record RefreshTokenCommand
{
    public required string RefreshToken { get; init; }
}
