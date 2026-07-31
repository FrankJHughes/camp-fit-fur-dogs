using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Middleware.Authorization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankIdentityApiMiddlewareAuthorization(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<RequireAuthenticatedUserMiddleware>();
    }
}
