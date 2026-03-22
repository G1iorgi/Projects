using Shopping.Application;
using Shopping.Infrastructure;
using Shopping.Infrastructure.DbContexts;
using Shopping.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

if (app.Configuration.GetValue("NeedToRunMigration", false))
{
    await Extensions.RunMigration(app.Services.GetRequiredService<ShoppingDbContextMaster>());
}

app.UseMiddlewares();

await app.RunAsync();
