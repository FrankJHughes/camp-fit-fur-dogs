#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Cors;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankCoreApiMiddlewareOriginLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OriginLoggingMiddleware>();
    }
}
