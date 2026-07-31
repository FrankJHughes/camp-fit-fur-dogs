using Frank.Identity.Api.Middleware.Authorization;
using Frank.Identity.Api.Middleware.Observations;
using Frank.Identity.Api.Middleware.Sessions;
using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Platform;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseFrankIdentityApiPlatform(this WebApplication app)
    {
        app.UseFrankIdentityApiMiddlewareObservations();
        app.UseAuthentication();
        app.UseForwardedHeaders();
        app.UseFrankIdentityApiMiddlewareSessionValidation();
        app.UseAuthorization();
        app.UseFrankIdentityApiMiddlewareAuthorization();

        return app;
    }
}
