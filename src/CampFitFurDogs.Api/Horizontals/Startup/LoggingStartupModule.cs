using Microsoft.AspNetCore.HttpLogging;

namespace CampFitFurDogs.Api.Horizontals.Startup;

[StartupModule(10)]
public static class LoggingStartupModule
{
    public static void Add(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPath |
                                    HttpLoggingFields.RequestMethod |
                                    HttpLoggingFields.ResponseStatusCode;
        });
    }

    public static void Use(IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Startup.Logging");

        if (env.IsDevelopment())
        {
            app.UseHttpLogging();
            logger.LogInformation("HTTP logging enabled for Development environment.");
        }

        var allowedOrigins = CorsStartupModule.AllowedOrigins;
        if (allowedOrigins is { Count: > 0 })
        {
            logger.LogInformation("CORS allowed origins: {Origins}", allowedOrigins);
        }
        else
        {
            logger.LogWarning("CORS allowed origins are not initialized or empty at startup.");
        }
    }
}
