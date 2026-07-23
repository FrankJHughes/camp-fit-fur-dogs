#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Identity.Api.Middleware.Observations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankIdentityApiMiddlewareObservations(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<ObservationInstrumentationMiddleware>()
            ;
    }
}
