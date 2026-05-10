using Core.Application.Aggregates.UserAggregate;
using Core.Application.Aggregates.UserAggregate.Commands;
using Core.Application.Aggregates.UserAggregate.Responses;
using Core.Domain;
using Core.Domain.Aggregates.IdentityAggregate;
using Core.Domain.Aggregates.IdentityAggregate.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SharedKernel.Exceptions.Identity;

namespace Core.Application.UnitTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<IOptions<TokenValidationOptions>> _jwtOptionsMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _jwtOptionsMock = new Mock<IOptions<TokenValidationOptions>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Setup JWT options before creating service
        _jwtOptionsMock.Setup(o => o.Value).Returns(new TokenValidationOptions
        {
            Secret = "supersecretkeythatislongenoughforjwttokens",
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        });

        _userService = new UserService(_userManagerMock.Object, _jwtOptionsMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_Should_Succeed()
    {
        // Arrange
        const string username = "testuser";
        const string email = "test@example.com";
        const string password = "Password123!";

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync((IdentityUser?)null);
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        var command = new RegisterUserCommand
        {
            Username = username,
            Email = email,
            Password = password
        };

        // Act
        await _userService.RegisterAsync(command);

        // Assert
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CreateAsync(It.Is<IdentityUser>(u => u.UserName == username && u.Email == email), password), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_Exception_When_User_Already_Exists()
    {
        // Arrange
        const string username = "existinguser";
        const string email = "existing@example.com";
        const string password = "Password123!";

        var existingUser = new IdentityUser { UserName = username, Email = email };
        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(existingUser);

        var command = new RegisterUserCommand
        {
            Username = username,
            Email = email,
            Password = password
        };

        // Act
        async Task Act() => await _userService.RegisterAsync(command);

        // Assert
        await Assert.ThrowsAsync<UserAlreadyExistsException>(Act);
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_Exception_When_Creation_Fails()
    {
        // Arrange
        const string username = "testuser";
        const string email = "test@example.com";
        const string password = "Password123!";

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync((IdentityUser?)null);
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var command = new RegisterUserCommand
        {
            Username = username,
            Email = email,
            Password = password
        };

        // Act
        async Task Act() => await _userService.RegisterAsync(command);

        // Assert
        await Assert.ThrowsAsync<UserCreationFailedException>(Act);
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), password), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _userService.RegisterAsync(null!));
    }

    [Fact]
    public async Task LoginAsync_Should_Succeed_With_New_Refresh_Token()
    {
        // Arrange
        const string username = "testuser";
        const string password = "Password123!";
        var user = new IdentityUser { Id = "userId", UserName = username, Email = "test@example.com" };

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, password))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);
        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(0));

        var command = new LoginUserCommand
        {
            Username = username,
            Password = password
        };

        // Act
        var result = await _userService.LoginAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Succeed_With_Existing_Valid_Refresh_Token()
    {
        // Arrange
        const string username = "testuser";
        const string password = "Password123!";
        var user = new IdentityUser { Id = "userId", UserName = username, Email = "test@example.com" };
        var existingRefreshToken = RefreshToken.Create("existingToken", user.Id);

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, password))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);

        var command = new LoginUserCommand
        {
            Username = username,
            Password = password
        };

        // Act
        var result = await _userService.LoginAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal("existingToken", result.RefreshToken);
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Should_Succeed_With_New_Refresh_Token_When_Existing_Is_Invalid()
    {
        // Arrange
        const string username = "testuser";
        const string password = "Password123!";
        var user = new IdentityUser { Id = "userId", UserName = username, Email = "test@example.com" };
        var existingRefreshToken = RefreshToken.Create("existingToken", user.Id);
        // Make it expired
        var expirationProperty = typeof(RefreshToken).GetProperty("Expiration", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        expirationProperty!.SetValue(existingRefreshToken, DateTimeOffset.UtcNow.AddDays(-1));

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, password))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);
        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(0));

        var command = new LoginUserCommand
        {
            Username = username,
            Password = password
        };

        // Act
        var result = await _userService.LoginAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);
        Assert.NotEqual("existingToken", result.RefreshToken); // Should be new token
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_Exception_When_User_Not_Found()
    {
        // Arrange
        const string username = "nonexistent";
        const string password = "Password123!";

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync((IdentityUser?)null);

        var command = new LoginUserCommand
        {
            Username = username,
            Password = password
        };

        // Act
        async Task Act() => await _userService.LoginAsync(command);

        // Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(Act);
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_Exception_When_Password_Invalid()
    {
        // Arrange
        const string username = "testuser";
        const string password = "WrongPassword";
        var user = new IdentityUser { Id = "userId", UserName = username, Email = "test@example.com" };

        _userManagerMock.Setup(um => um.FindByNameAsync(username))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, password))
            .ReturnsAsync(false);

        var command = new LoginUserCommand
        {
            Username = username,
            Password = password
        };

        // Act
        async Task Act() => await _userService.LoginAsync(command);

        // Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(Act);
        _userManagerMock.Verify(um => um.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(um => um.CheckPasswordAsync(user, password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _userService.LoginAsync(null!));
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Succeed()
    {
        // Arrange
        const string refreshTokenValue = "validRefreshToken";
        var user = new IdentityUser { Id = "userId", UserName = "testuser", Email = "test@example.com" };
        var refreshToken = RefreshToken.Create(refreshTokenValue, user.Id);

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _userManagerMock.Setup(um => um.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.Update(It.IsAny<RefreshToken>()));
        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(0));

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenValue
        };

        // Act
        var result = await _userService.RefreshTokenAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(um => um.FindByIdAsync(user.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.Update(refreshToken), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.CreateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_Exception_When_Refresh_Token_Is_Null()
    {
        // Arrange
        const string refreshTokenValue = "invalidToken";

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenValue
        };

        // Act
        async Task Act() => await _userService.RefreshTokenAsync(command);

        // Assert
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(Act);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_Exception_When_Refresh_Token_Is_Revoked()
    {
        // Arrange
        const string refreshTokenValue = "revokedToken";
        var refreshToken = RefreshToken.Create(refreshTokenValue, "userId");
        refreshToken.MarkAsRevoked();

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenValue
        };

        // Act
        async Task Act() => await _userService.RefreshTokenAsync(command);

        // Assert
        await Assert.ThrowsAsync<RevokedRefreshTokenException>(Act);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_Exception_When_Refresh_Token_Is_Expired()
    {
        // Arrange
        const string refreshTokenValue = "expiredToken";
        var refreshToken = RefreshToken.Create(refreshTokenValue, "userId");

        // Use reflection to set Expiration to past
        var expirationProperty = typeof(RefreshToken).GetProperty("Expiration", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        expirationProperty!.SetValue(refreshToken, DateTimeOffset.UtcNow.AddDays(-1));

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenValue
        };

        // Act
        async Task Act() => await _userService.RefreshTokenAsync(command);

        // Assert
        await Assert.ThrowsAsync<ExpiredRefreshTokenException>(Act);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_Exception_When_User_Not_Found()
    {
        // Arrange
        const string refreshTokenValue = "validToken";
        var refreshToken = RefreshToken.Create(refreshTokenValue, "userId");

        _unitOfWorkMock.Setup(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _userManagerMock.Setup(um => um.FindByIdAsync("userId"))
            .ReturnsAsync((IdentityUser?)null);

        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshTokenValue
        };

        // Act
        async Task Act() => await _userService.RefreshTokenAsync(command);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
        _unitOfWorkMock.Verify(uow => uow.RefreshTokens.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(um => um.FindByIdAsync("userId"), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Throw_When_Command_Is_Null()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _userService.RefreshTokenAsync(null!));
    }
}
