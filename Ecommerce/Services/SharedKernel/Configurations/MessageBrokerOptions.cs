namespace SharedKernel.Configurations;

public class MessageBrokerOptions
{
    public const string Key = "MessageBrokerOptions";

    public required string Host { get; init; }

    public required string UserName { get; init; }

    public required string Password { get; init; }

    public required string QueueName { get; init; }
}
