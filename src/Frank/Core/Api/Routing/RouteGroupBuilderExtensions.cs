using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Frank.Core.Api.Routing;

/// <summary>
/// Provides unified endpoint filtering support for Minimal API endpoint groups.
/// This enables cross‑cutting behaviors (validation, logging, metrics, etc.)
/// to be applied consistently across all endpoints in a group.
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// Adds a global endpoint filter factory to the specified endpoint group.
    /// All endpoints mapped within the group will execute the unified filter pipeline.
    /// </summary>
    /// <param name="group">The endpoint group to configure.</param>
    public static void AddEndpointFiltering(this RouteGroupBuilder group)
    {
        group.AddEndpointFilterFactory((context, next) =>
        {
            // This is the unified pipeline hook.
            // ANY filter can plug in here:
            // - Validation
            // - Logging
            // - Metrics
            // - Correlation IDs
            // - Domain-specific filters
            // - Anything else you add later

            return async invocationContext =>
            {
                // Pre-execution hook (optional)
                // Example: Logging
                // var logger = invocationContext.HttpContext.RequestServices
                //     .GetRequiredService<ILoggerFactory>()
                //     .CreateLogger("EndpointFiltering");
                // logger.LogInformation("Executing endpoint {Endpoint}", context.MethodInfo.Name);

                // Execute next filter or endpoint handler
                var result = await next(invocationContext);

                // Post-execution hook (optional)
                // logger.LogInformation("Completed endpoint {Endpoint}", context.MethodInfo.Name);

                return result;
            };
        });
    }
}
