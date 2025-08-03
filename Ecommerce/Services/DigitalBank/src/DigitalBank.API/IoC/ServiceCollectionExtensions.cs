namespace DigitalBank.API.IoC;

public static class ServiceCollectionExtensions
{
    public static void AddDigitalBankServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddServices();
        services.AddApiServices();
    }

    public static void UseDigitalBankServices(this IApplicationBuilder app)
    {
        app.UseSwaggerAndSwaggerUI();
    }

    public static void UseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => Results.Ok("Digital Bank API"));

        app.AddCreditCardEndpoints();
        app.AddTransactionEndpoints();
    }
}
