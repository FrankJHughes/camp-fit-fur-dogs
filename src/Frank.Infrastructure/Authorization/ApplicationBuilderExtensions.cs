using Microsoft.AspNetCore.Builder;

namespace Frank.Infrastructure.Authorization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankAuthorization(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<RequireAuthenticatedUserMiddleware>();
    }
}
