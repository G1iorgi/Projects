using Ardalis.GuardClauses;
using Core.Application.Aggregates.UserAggregate.Commands;

namespace Core.Presentation.Request.UserRequest;

public record RegisterUserRequest
{
    public required string Username { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public static RegisterUserCommand ToCommand(RegisterUserRequest? request)
    {
        Guard.Against.Null(request);
        Guard.Against.NullOrWhiteSpace(request.Username);
        Guard.Against.NullOrWhiteSpace(request.Email);
        Guard.Against.NullOrWhiteSpace(request.Password);

        return new RegisterUserCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };
    }
}
