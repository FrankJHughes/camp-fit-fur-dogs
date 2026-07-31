using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Api.Platform.Swagger;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseFrankCoreApiPlatformSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}
