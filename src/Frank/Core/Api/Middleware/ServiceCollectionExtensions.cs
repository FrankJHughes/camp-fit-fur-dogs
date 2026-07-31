using Frank.Core.Api.Middleware.Observations;
using Frank.Core.Api.Middleware.SecurityHeaders;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Middleware;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApiMiddleware(this IServiceCollection services)
    {
        return services
            .AddFrankCoreApiObservations()
            .AddFrankCoreApiSecurityHeaders()
            ;
    }
}
