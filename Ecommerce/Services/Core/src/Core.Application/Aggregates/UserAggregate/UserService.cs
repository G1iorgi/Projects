using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ardalis.GuardClauses;
using Core.Application.Aggregates.UserAggregate.Commands;
using Core.Application.Aggregates.UserAggregate.Responses;
using Core.Domain;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.IdentityAggregate.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Exceptions.Identity;

namespace Core.Application.Aggregates.UserAggregate;

public class UserService(
    UserManager<IdentityUser> userManager,
    IOptions<TokenValidationOptions> jwtOptions,
    IUnitOfWork unitOfWork)
{
    private readonly TokenValidationOptions _jwtConfig = jwtOptions.Value;

    public async Task RegisterAsync(RegisterUserCommand command)
    {
        Guard.Against.Null(command);

        var userExists = await userManager.FindByNameAsync(command.Username);
        if (userExists != null)
            throw new UserAlreadyExistsException(command.Username);

        var user = new IdentityUser
        {
            UserName = command.Username,
            Email = command.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new UserCreationFailedException(errors);
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        var user = await userManager.FindByNameAsync(command.Username);
        if (user == null)
            throw new InvalidCredentialsException();

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
            throw new InvalidCredentialsException();

        // Generate JWT access token
        var accessToken = GenerateJwtToken(user);

        // Check if the user already has a valid refresh token
        var existingRefreshToken = await unitOfWork.RefreshTokens.GetByUserIdAsync(user.Id, cancellationToken);

        string refreshToken;
        if (RefreshTokenIsValid(existingRefreshToken))
        {
            // If a valid refresh token exists, use it
            refreshToken = existingRefreshToken!.Token;
        }
        else
        {
            // Otherwise, generate a new refresh token
            refreshToken = GenerateRefreshToken();

            // Create refresh token entity
            var refreshTokenEntity = RefreshToken.Create(refreshToken, user.Id);

            await unitOfWork.RefreshTokens.CreateAsync(refreshTokenEntity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new LoginResponse { Token = accessToken, RefreshToken = refreshToken };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command);

        // Fetch the refresh token from the database
        var refreshToken = await unitOfWork.RefreshTokens.GetByTokenAsync(command.RefreshToken, cancellationToken);

        ThrowIfRefreshTokenIsInvalid(refreshToken);

        // Generate new tokens
        var user = await userManager.FindByIdAsync(refreshToken!.UserId);
        if (user == null)
            throw new UserNotFoundException();

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        // Mark the old refresh token as used and revoked
        refreshToken.MarkAsRevoked();
        unitOfWork.RefreshTokens.Update(refreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var newRefreshTokenEntity = RefreshToken.Create(newRefreshToken, user.Id);
        await unitOfWork.RefreshTokens.CreateAsync(newRefreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse { Token = newAccessToken, RefreshToken = newRefreshToken };
    }

    private string GenerateJwtToken(IdentityUser? user)
    {
        Guard.Against.Null(user);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // create the token
        var token = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTimeOffset.UtcNow.AddMinutes(10).UtcDateTime,
            SigningCredentials = credentials,
        };

        return new JsonWebTokenHandler().CreateToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static bool RefreshTokenIsValid(RefreshToken? refreshToken)
    {
        return refreshToken != null &&
               refreshToken.Expiration > DateTimeOffset.UtcNow &&
               refreshToken is { IsRevoked: false };
    }

    private static void ThrowIfRefreshTokenIsInvalid(RefreshToken? refreshToken)
    {
        if (refreshToken == null)
            throw new InvalidRefreshTokenException();
        if (refreshToken.IsRevoked)
            throw new RevokedRefreshTokenException();
        if (refreshToken.Expiration < DateTimeOffset.UtcNow)
            throw new ExpiredRefreshTokenException();
    }
}
