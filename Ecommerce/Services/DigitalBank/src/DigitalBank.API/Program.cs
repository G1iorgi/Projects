using DigitalBank.API;
using DigitalBank.API.IoC;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDigitalBankServices(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue("NeedToRunMigration", true))
{
    await Extensions.RunMigration(app);
}

app.UseDigitalBankServices();
app.UseEndpoints();

await app.RunAsync();
