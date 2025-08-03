using Ardalis.GuardClauses;
using Core.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure;

public static class Extensions
{
    public static async Task RunMigration(CoreDbContextMaster dbContext)
    {
        Guard.Against.Null(dbContext);

        try
        {
            await dbContext.Database.OpenConnectionAsync();
            await dbContext.Database.MigrateAsync();
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
}
