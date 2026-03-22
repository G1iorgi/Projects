using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Core.Presentation.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.Middlewares.GlobalExceptionHandlingMiddleware;

namespace Core.Presentation;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services)
    {
        Guard.Against.Null(services);

        services.AddControllers();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        services.AddHttpContextAccessor();
        services.AddCustomOptions();
        services.AddEndpointsApiExplorer();
        services.AddSwagger();
        services.AddMinimalEndpoints();
        services.AddCoreAuthentication();
        services.AddAuthorization();
    }

    public static void UseMiddlewares(this WebApplication app)
    {
        Guard.Against.Null(app);

        app.UseMiddleware<GlobalExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API V1"); });
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMinimalEndpoints();
        app.UseHttpsRedirection();
    }
}
