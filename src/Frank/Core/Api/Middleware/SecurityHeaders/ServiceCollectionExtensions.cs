using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Middleware.SecurityHeaders;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        services.AddTransient<SecurityHeadersMiddleware>();
        return services;
    }
}
