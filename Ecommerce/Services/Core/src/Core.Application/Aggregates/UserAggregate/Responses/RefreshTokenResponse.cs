namespace Core.Application.Aggregates.UserAggregate.Responses;

public record RefreshTokenResponse
{
    public required string Token { get; init; }

    public required string RefreshToken { get; init; }
}
