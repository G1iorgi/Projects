using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Identity;

public sealed class UserCreationFailedException(string errors) : ValidationException(
    $"User creation failed: {errors}",
    "USER_CREATION_FAILED");
