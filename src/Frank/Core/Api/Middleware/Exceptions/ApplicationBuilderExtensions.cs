#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Exceptions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankCoreApiMiddlewareExceptions(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
