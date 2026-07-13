#nullable enable
using Frank.Core.Infrastructure.Observations.Http;
using Microsoft.AspNetCore.Builder;

namespace Frank.Core.Infrastructure.Observations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFrankObservations(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InboundObservationContextMiddleware>()
            .UseMiddleware<ObservationInstrumentationMiddleware>();
    }
}
