using Payment.Application;
using Payment.Infrastructure;
using Payment.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

app.UseMiddlewares();

await app.RunAsync();
