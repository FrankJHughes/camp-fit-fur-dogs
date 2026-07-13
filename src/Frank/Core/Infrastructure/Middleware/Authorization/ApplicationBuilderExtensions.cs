using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Infrastructure.Middleware.Authorization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseOidcorization(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<RequireAuthenticatedUserMiddleware>();
    }
}
