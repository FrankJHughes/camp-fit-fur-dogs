using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.HttpLogging;

namespace CampFitFurDogs.Api.Horizontals.Startup.Modules;

[StartupModule(10)]
public class LoggingStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPath |
                                    HttpLoggingFields.RequestMethod |
                                    HttpLoggingFields.ResponseStatusCode;
        });
    }

    public void Use(WebApplication app)
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var logger = app.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Startup.Logging");

        if (env.IsDevelopment())
        {
            app.UseHttpLogging();
            logger.LogInformation("HTTP logging enabled for Development environment.");
        }
    }
}
