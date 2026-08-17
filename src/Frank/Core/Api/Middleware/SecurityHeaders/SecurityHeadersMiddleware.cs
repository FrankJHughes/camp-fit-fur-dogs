#nullable enable
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Frank.Core.Api.Middleware.SecurityHeaders;

/// <summary>
/// Middleware that applies a hardened set of security headers to all HTTP
/// responses, following modern OWASP recommendations and strict baseline
/// Content Security Policy (CSP) defaults.
/// <para>
/// This middleware ensures that every response includes defensive headers
/// protecting against MIME sniffing, clickjacking, XSS, cross-origin attacks,
/// and unsafe resource embedding.
/// It also applies a strict CSP suitable for APIs and backend services.
/// </para>
/// </summary>
public sealed class SecurityHeadersMiddleware : IMiddleware
{
    /// <summary>
    /// Adds security headers to the outgoing HTTP response if they are not
    /// already present.
    /// <para>
    /// The middleware applies:
    /// <list type="bullet">
    /// <item><description><c>X-Content-Type-Options</c> — prevents MIME sniffing</description></item>
    /// <item><description><c>X-Frame-Options</c> — blocks clickjacking</description></item>
    /// <item><description><c>X-XSS-Protection</c> — disables legacy XSS filters</description></item>
    /// <item><description><c>Referrer-Policy</c> — limits referrer leakage</description></item>
    /// <item><description><c>Permissions-Policy</c> — disables sensitive browser APIs</description></item>
    /// <item><description><c>Cross-Origin-Opener-Policy</c></description></item>
    /// <item><description><c>Cross-Origin-Embedder-Policy</c></description></item>
    /// <item><description><c>Cross-Origin-Resource-Policy</c></description></item>
    /// <item><description><c>Content-Security-Policy</c> — strict modern baseline</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Response.Headers;

        // OWASP + modern hardened defaults
        SetIfMissing(headers, "X-Content-Type-Options", "nosniff");
        SetIfMissing(headers, "X-Frame-Options", "DENY");
        SetIfMissing(headers, "X-XSS-Protection", "0");
        SetIfMissing(headers, "Referrer-Policy", "strict-origin-when-cross-origin");
        SetIfMissing(headers, "Permissions-Policy", "geolocation=(), microphone=(), camera=(), payment=(), usb=()");
        SetIfMissing(headers, "Cross-Origin-Opener-Policy", "same-origin");
        SetIfMissing(headers, "Cross-Origin-Embedder-Policy", "require-corp");
        SetIfMissing(headers, "Cross-Origin-Resource-Policy", "same-origin");

        // CSP — strict modern baseline
        var sb = new StringBuilder();
        sb.Append("default-src 'self'; ");
        sb.Append("script-src 'self'; ");
        sb.Append("style-src 'self'; ");
        sb.Append("img-src 'self' data:; ");
        sb.Append("font-src 'self'; ");
        sb.Append("connect-src 'self'; ");
        sb.Append("frame-ancestors 'none'; ");
        sb.Append("object-src 'none'; ");
        sb.Append("base-uri 'self'; ");
        sb.Append("form-action 'self'");
        SetIfMissing(headers, "Content-Security-Policy", sb.ToString());

        return next(context);
    }

    /// <summary>
    /// Sets a header only if it is not already present on the response.
    /// </summary>
    private static void SetIfMissing(IHeaderDictionary headers, string key, string value)
    {
        if (!headers.ContainsKey(key))
        {
            headers[key] = value;
        }
    }
}
