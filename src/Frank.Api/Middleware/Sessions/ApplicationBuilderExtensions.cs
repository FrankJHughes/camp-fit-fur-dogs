using Microsoft.AspNetCore.Builder;

namespace Frank.Api.Middleware.Sessions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SessionValidationMiddleware>();
    }
}
