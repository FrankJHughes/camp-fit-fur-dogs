#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Api.Exceptions.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankExceptions(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
