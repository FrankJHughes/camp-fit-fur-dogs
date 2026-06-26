using System.Security.Cryptography;
using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability.Http;

public sealed class OutboundObservabilityContextHandler : DelegatingHandler
{
    private readonly IRequestObservabilityContext _context;

    public OutboundObservabilityContextHandler(IRequestObservabilityContext context)
    {
        _context = context;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // -----------------------------
        // 1. Correlation Headers
        // -----------------------------
        if (!request.Headers.Contains("X-Correlation-ID"))
            request.Headers.Add("X-Correlation-ID", _context.CorrelationId);

        if (!request.Headers.Contains("X-Channel"))
            request.Headers.Add("X-Channel", _context.Channel);

        if (!request.Headers.Contains("X-Agent"))
            request.Headers.Add("X-Agent", _context.Agent);

        // -----------------------------
        // 2. W3C TraceContext: traceparent
        // -----------------------------
        if (!request.Headers.Contains("traceparent"))
        {
            var traceId = NormalizeTraceId(_context.CorrelationId);
            var spanId = GenerateSpanId();
            const string version = "00";
            const string flags = "01"; // sampled

            var traceparent = $"{version}-{traceId}-{spanId}-{flags}";
            request.Headers.Add("traceparent", traceparent);
        }

        return base.SendAsync(request, cancellationToken);
    }

    private static string NormalizeTraceId(string correlationId)
    {
        // If correlationId is a GUID, convert to 32-char hex
        if (Guid.TryParse(correlationId, out var guid))
            return guid.ToString("N");

        // Otherwise hash it to 16 bytes
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(correlationId));
        return Convert.ToHexString(bytes.AsSpan(0, 16)).ToLowerInvariant();
    }

    private static string GenerateSpanId()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
