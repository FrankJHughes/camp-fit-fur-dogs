#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Api.Middleware.Observations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankObservations(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InboundObservationContextMiddleware>()
            .UseMiddleware<ObservationInstrumentationMiddleware>();
    }
}
