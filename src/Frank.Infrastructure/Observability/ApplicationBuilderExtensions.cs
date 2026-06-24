#nullable enable
using Frank.Infrastructure.Observability.Http;
using Microsoft.AspNetCore.Builder;

namespace Frank.Infrastructure.Observability;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        return app.UseMiddleware<InboundTraceContextMiddleware>()
            .UseMiddleware<ObservabilityMiddleware>();
    }
}
