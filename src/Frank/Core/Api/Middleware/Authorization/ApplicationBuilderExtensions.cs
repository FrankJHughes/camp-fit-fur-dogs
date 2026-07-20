using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Authorization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseOidcorization(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<RequireCurrentUserMiddleware>();
    }
}
