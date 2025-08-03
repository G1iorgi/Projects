using Ardalis.GuardClauses;
using DigitalBank.API.DB;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.API;

public static class Extensions
{
    public static async Task RunMigration(WebApplication app)
    {
        Guard.Against.Null(app);

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DigitalBankDbContext>();
        try
        {
            await dbContext.Database.OpenConnectionAsync();
            await dbContext.Database.MigrateAsync();

            await ExecuteInsertInitialData(app, dbContext);

            Console.WriteLine("Migration completed successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.InnerException != null ? e.InnerException.Message : e.Message);
            throw;
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
            await dbContext.DisposeAsync();
        }
    }

    // execute InsertInitialData.sql script
    private static async Task ExecuteInsertInitialData(WebApplication app, DigitalBankDbContext dbContext)
    {
        Guard.Against.Null(app);

        var script = await File.ReadAllTextAsync("DB/Scripts/InsertInitialData.sql");

        if (!string.IsNullOrWhiteSpace(script))
        {
            await dbContext.Database.ExecuteSqlRawAsync(script);
        }
    }
}
