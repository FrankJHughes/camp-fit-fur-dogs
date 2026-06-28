using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CampFitFurDogs.Api.Horizontals.Cors.Middleware;

public sealed class OriginLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OriginLoggingMiddleware> _logger;
    private readonly ICorsPolicyProvider _policyProvider;

    public OriginLoggingMiddleware(
        RequestDelegate next,
        ILogger<OriginLoggingMiddleware> logger,
        ICorsPolicyProvider policyProvider)
    {
        _next = next;
        _logger = logger;
        _policyProvider = policyProvider;
    }

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
