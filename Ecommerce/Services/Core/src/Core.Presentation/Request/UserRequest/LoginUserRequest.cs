using Ardalis.GuardClauses;
using Core.Application.Aggregates.UserAggregate.Commands;

namespace Core.Presentation.Request.UserRequest;

public record LoginUserRequest
{
    public required string Username { get; init; }

    public required string Password { get; init; }

    public static LoginUserCommand ToCommand(LoginUserRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.Username);
        Guard.Against.NullOrWhiteSpace(request.Password);

        return new LoginUserCommand
        {
            Username = request.Username,
            Password = request.Password
        };
    }
}
