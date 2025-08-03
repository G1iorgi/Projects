using DigitalBank.API.DB;
using DigitalBank.API.Repositories;
using DigitalBank.API.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.API.IoC;

public static class RegisterServices
{
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DigitalBankDbContext>(
            options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString(nameof(DigitalBankDbContext)),
                    npgsqlBuilder => npgsqlBuilder.MigrationsHistoryTable(DigitalBankDbContext.MigrationHistoryTable));
                options.UseLazyLoadingProxies();
            });
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CreditCardRepository>();
        services.AddScoped<TransactionRepository>();
        services.AddScoped<BalanceRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<CreditCardService>();
        services.AddScoped<TransactionService>();
    }

    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddSwaggerGen();
    }
}
