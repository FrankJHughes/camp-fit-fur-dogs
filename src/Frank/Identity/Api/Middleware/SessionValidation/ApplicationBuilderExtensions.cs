using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Middleware.Sessions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankIdentityApiMiddlewareSessionValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SessionValidationMiddleware>();
    }
}
