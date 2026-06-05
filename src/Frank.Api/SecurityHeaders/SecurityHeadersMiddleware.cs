using System.Text;
using Microsoft.AspNetCore.Http;

namespace Frank.Api.SecurityHeaders;

public sealed class SecurityHeadersMiddleware : IMiddleware
{
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

    private static void SetIfMissing(IHeaderDictionary headers, string key, string value)
    {
        if (!headers.ContainsKey(key))
        {
            headers[key] = value;
        }
    }
}
