#nullable enable
using Microsoft.AspNetCore.Builder;

namespace Frank.Api.Observability;

public static class ObservabilityApplicationBuilderExtensions
{
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ObservabilityMiddleware>();
    }
}
