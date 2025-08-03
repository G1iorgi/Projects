using Swashbuckle.AspNetCore.SwaggerUI;

namespace DigitalBank.API.IoC;

public static class UseServices
{
    public static void UseSwaggerAndSwaggerUI(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(DocExpansion.None);
                c.DisplayRequestDuration();
                c.EnableTryItOutByDefault();
            });

        app.UseHttpsRedirection();
    }
}
