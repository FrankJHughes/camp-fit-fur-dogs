using Frank.Core.Api.Middleware;
using Frank.Core.Api.Platform.Cors;
using Frank.Core.Api.Platform.Logging;
using Frank.Core.Api.Platform.Swagger;
using Frank.Core.Application;
using Frank.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApiPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddFrankCoreApiPlatformCors(configuration)
            .AddFrankCoreApiPlatformLogging()
            .AddFrankCoreApiPlatformSwagger()
            .AddFrankCoreApplication()
            .AddFrankCoreInfrastructure()
            .AddFrankCoreApiMiddleware();
        ;
    }
}
