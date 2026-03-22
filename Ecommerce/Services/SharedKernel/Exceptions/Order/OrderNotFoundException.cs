using SharedKernel.Exceptions.HttpResponseExceptions;

namespace SharedKernel.Exceptions.Order;

public sealed class OrderNotFoundException(int orderId) : NotFoundException(
    $"Order with ID {orderId} was not found.",
    "ORDER_NOT_FOUND");
