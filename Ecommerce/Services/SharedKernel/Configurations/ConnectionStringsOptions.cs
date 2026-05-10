namespace SharedKernel.Configurations;

public class ConnectionStringsOptions
{
    public const string Key = "ConnectionStrings";

    public required string ShoppingDbContextMaster { get; init; }
}
