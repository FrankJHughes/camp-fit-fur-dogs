using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Frank.Core.Api.Middleware.Cors;

/// <summary>
/// Middleware that logs detailed CORS activity for both simple requests and
/// preflight (OPTIONS) requests.
/// <para>
/// This middleware inspects the incoming <c>Origin</c> header, evaluates the
/// configured CORS policy via <see cref="ICorsPolicyProvider"/>, and logs whether
/// the origin is allowed or blocked.
/// It provides visibility into CORS behavior during development, diagnostics,
/// and production monitoring.
/// </para>
/// </summary>
public sealed class OriginLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OriginLoggingMiddleware> _logger;
    private readonly ICorsPolicyProvider _policyProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OriginLoggingMiddleware"/>
    /// class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger used to record CORS activity.</param>
    /// <param name="policyProvider">
    /// The CORS policy provider used to evaluate allowed origins.
    /// </param>
    public OriginLoggingMiddleware(
        RequestDelegate next,
        ILogger<OriginLoggingMiddleware> logger,
        ICorsPolicyProvider policyProvider)
    {
        _next = next;
        _logger = logger;
        _policyProvider = policyProvider;
    }

    /// <summary>
    /// Processes the incoming HTTP request, evaluates its CORS origin, and logs
    /// whether the request is allowed or blocked according to the active CORS
    /// policy.
    /// <para>
    /// For preflight requests (OPTIONS + <c>Access-Control-Request-Method</c>),
    /// the middleware logs the requested method and headers.
    /// For simple requests, it logs only the origin, method, and path.
    /// </para>
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var origin = context.Request.Headers["Origin"].ToString();
        if (string.IsNullOrWhiteSpace(origin))
        {
            await _next(context);
            return;
        }

        var policy = await _policyProvider.GetPolicyAsync(context, null);
        var allowedOrigins = policy?.Origins ?? Array.Empty<string>();
        var isAllowed = allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);

        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        var isPreflight = HttpMethods.IsOptions(method) &&
                          !string.IsNullOrWhiteSpace(context.Request.Headers["Access-Control-Request-Method"]);

        if (isPreflight)
        {
            var requestedMethod = context.Request.Headers["Access-Control-Request-Method"].ToString();
            var requestedHeaders = context.Request.Headers["Access-Control-Request-Headers"].ToString();

            if (isAllowed)
            {
                _logger.LogInformation(
                    "CORS preflight allowed. Origin={Origin}, Method={Method}, RequestMethod={RequestedMethod}, RequestHeaders={RequestedHeaders}, Path={Path}",
                    origin, method, requestedMethod, requestedHeaders, path);
            }
            else
            {
                _logger.LogWarning(
                    "CORS preflight blocked. Origin={Origin}, Method={Method}, RequestMethod={RequestedMethod}, RequestHeaders={RequestedHeaders}, Path={Path}",
                    origin, method, requestedMethod, requestedHeaders, path);
            }
        }
        else
        {
            if (isAllowed)
            {
                _logger.LogInformation(
                    "CORS request allowed. Origin={Origin}, Method={Method}, Path={Path}",
                    origin, method, path);
            }
            else
            {
                _logger.LogWarning(
                    "CORS request blocked. Origin={Origin}, Method={Method}, Path={Path}",
                    origin, method, path);
            }
        }

        await _next(context);
    }
}
