using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApiPlatformLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields =
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.ResponseStatusCode;
        });

        return services;
    }
}
