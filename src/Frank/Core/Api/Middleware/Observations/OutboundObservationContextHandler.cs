using System.Security.Cryptography;
using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Api.Middleware.Observations;

/// <summary>
/// A delegating handler that enriches outbound HTTP requests with correlation,
/// channel, agent, and W3C TraceContext headers derived from the current
/// <see cref="IRequestObservationContext"/>.
/// <para>
/// This ensures that outbound calls made by the API participate in distributed
/// tracing, propagate correlation identifiers, and carry consistent observability
/// metadata across service boundaries.
/// </para>
/// </summary>
public sealed class OutboundObservationContextHandler : DelegatingHandler
{
    private readonly IRequestObservationContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboundObservationContextHandler"/>
    /// class using the provided observation context.
    /// </summary>
    /// <param name="context">
    /// The current request's observation context, containing correlation ID,
    /// channel, agent, and other metadata.
    /// </param>
    public OutboundObservationContextHandler(IRequestObservationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds correlation headers and W3C <c>traceparent</c> headers to the outbound
    /// HTTP request before sending it.
    /// <para>
    /// The following headers are added when not already present:
    /// <list type="bullet">
    /// <item><description><c>X-Correlation-ID</c></description></item>
    /// <item><description><c>X-Channel</c></description></item>
    /// <item><description><c>X-Agent</c></description></item>
    /// <item><description><c>traceparent</c> (W3C TraceContext)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The <c>traceparent</c> header is generated using:
    /// <list type="bullet">
    /// <item><description>A normalized 32‑character trace ID derived from the correlation ID</description></item>
    /// <item><description>A randomly generated 16‑character span ID</description></item>
    /// <item><description>Version <c>00</c> and sampled flag <c>01</c></description></item>
    /// </list>
    /// </para>
    /// </summary>
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

    /// <summary>
    /// Normalizes the correlation ID into a valid W3C trace ID.
    /// <para>
    /// If the correlation ID is a GUID, it is converted to a 32‑character
    /// lowercase hexadecimal string.
    /// Otherwise, the correlation ID is hashed using SHA‑256 and the first
    /// 16 bytes (32 hex characters) are used as the trace ID.
    /// </para>
    /// </summary>
    private static string NormalizeTraceId(string correlationId)
    {
        // If correlationId is a GUID, convert to 32-char hex
        if (Guid.TryParse(correlationId, out var guid))
            return guid.ToString("N");

        // Otherwise hash it to 16 bytes
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(correlationId));
        return Convert.ToHexString(bytes.AsSpan(0, 16)).ToLowerInvariant();
    }

    /// <summary>
    /// Generates a random 16‑character hexadecimal span ID suitable for use in
    /// W3C TraceContext <c>traceparent</c> headers.
    /// </summary>
    private static string GenerateSpanId()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
