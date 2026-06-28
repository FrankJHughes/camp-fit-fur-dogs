#nullable enable
using Frank.Infrastructure.Observations.Http;
using Microsoft.AspNetCore.Builder;

namespace Frank.Infrastructure.Observations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankObservations(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InboundObservationContextMiddleware>()
            .UseMiddleware<ObservationInstrumentationMiddleware>();
    }
}
