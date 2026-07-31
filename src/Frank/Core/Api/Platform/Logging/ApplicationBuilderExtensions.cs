using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Frank.Core.Api.Platform.Logging;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseFrankCoreApiPlatformLogging(this WebApplication app)
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var logger = app.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Startup.Logging");

        if (env.IsDevelopment())
        {
            app.UseHttpLogging();
            logger.LogInformation("HTTP logging enabled for Development environment.");
        }

        return app;
    }
}
