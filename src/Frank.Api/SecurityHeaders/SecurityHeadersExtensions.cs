using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api.SecurityHeaders;

public static class SecurityHeadersExtensions
{
    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        services.AddTransient<SecurityHeadersMiddleware>();
        return services;
    }
}
