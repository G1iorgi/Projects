using Core.Application;
using Core.Infrastructure;
using Core.Infrastructure.DbContexts;
using Core.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

if (app.Configuration.GetValue("NeedToRunMigration", false))
{
    await Extensions.RunMigration(app.Services.GetRequiredService<CoreDbContextMaster>());
}

app.UseMiddlewares();

await app.RunAsync();
