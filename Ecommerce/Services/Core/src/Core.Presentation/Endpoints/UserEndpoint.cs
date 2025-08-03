using Core.Application.Aggregates.UserAggregate;
using Core.Presentation.Request.UserRequest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Core.Presentation.Endpoints;

public class UserEndpoint : IEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("api/users").WithTags("Users");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapPost("/refresh-token", RefreshToken);
    }

    private static async Task<IResult> Register(
        UserService userService,
        [FromBody] RegisterUserRequest? request)
    {
        var command = RegisterUserRequest.ToCommand(request);
        await userService.RegisterAsync(command);

        return Results.Ok();
    }

    private static async Task<IResult> Login(
        UserService userservice,
        [FromBody] LoginUserRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = LoginUserRequest.ToCommand(request);
        var result = await userservice.LoginAsync(command, cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> RefreshToken(
        UserService userService,
        [FromBody] RefreshTokenRequest? request,
        CancellationToken cancellationToken = default)
    {
        var command = RefreshTokenRequest.ToCommand(request);
        var result = await userService.RefreshTokenAsync(command, cancellationToken);

        return Results.Ok(result);
    }
}
