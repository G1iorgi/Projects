using Ardalis.GuardClauses;
using Core.Application.Aggregates.UserAggregate.Commands;

namespace Core.Presentation.Request.UserRequest;

public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }

    public static RefreshTokenCommand ToCommand(RefreshTokenRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.RefreshToken);

        return new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        };
    }
}
