using FluentValidation.TestHelper;
using Shopping.Application.Aggregates.WishlistAggregate.Commands.RemoveItem;

namespace Shopping.Application.UnitTests.WishlistTests.Commands;

public class RemoveWishlistItemCommandValidatorTests
{
    private readonly RemoveWishlistItemCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(1, "user-1");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithInvalidProductId_ShouldFail(int productId)
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(productId, "user-1");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductId)
            .WithErrorMessage("ProductId must be greater than 0.");
    }

    [Fact]
    public void Validate_WithPositiveProductId_ShouldPass()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(100, "user-1");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithLargeProductId_ShouldPass()
    {
        // Arrange
        var command = new RemoveWishlistItemCommand(int.MaxValue, "user-1");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

