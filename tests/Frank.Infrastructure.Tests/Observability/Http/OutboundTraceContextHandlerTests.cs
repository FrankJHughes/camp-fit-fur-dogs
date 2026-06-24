#nullable enable
using System.Net;
using Frank.Abstractions.Observability;
using Frank.Infrastructure.Observability.Http;
using Xunit;

namespace Frank.Infrastructure.Tests.Observability.Http;

public class OutboundTraceContextHandlerTests
{
    private sealed class TestObservabilityContext : IObservabilityContext
    {
        public string CorrelationId { get; init; } = "11111111-2222-3333-4444-555555555555";
        public string Channel { get; init; } = "test-channel";
        public string Agent { get; init; } = "test-agent";
        public string Environment { get; init; } = "Testing";
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public IReadOnlyDictionary<string, object?> Metadata { get; init; } = new Dictionary<string, object?>();
    }

    private sealed class TestMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    [Fact]
    public async Task Adds_correlation_headers_when_missing()
    {
        var context = new TestObservabilityContext();
        var inner = new TestMessageHandler();
        var handler = new OutboundTraceContextHandler(context) { InnerHandler = inner };
        var client = new HttpClient(handler);

        await client.GetAsync("http://example.com");

        Assert.Equal(context.CorrelationId, inner.LastRequest!.Headers.GetValues("X-Correlation-ID").Single());
        Assert.Equal(context.Channel, inner.LastRequest!.Headers.GetValues("X-Channel").Single());
        Assert.Equal(context.Agent, inner.LastRequest!.Headers.GetValues("X-Agent").Single());
    }

    [Fact]
    public async Task Does_not_overwrite_existing_correlation_headers()
    {
        var context = new TestObservabilityContext();
        var inner = new TestMessageHandler();
        var handler = new OutboundTraceContextHandler(context) { InnerHandler = inner };
        var client = new HttpClient(handler);

        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        request.Headers.Add("X-Correlation-ID", "existing");
        request.Headers.Add("X-Channel", "existing-channel");
        request.Headers.Add("X-Agent", "existing-agent");

        await client.SendAsync(request);

        Assert.Equal("existing", inner.LastRequest!.Headers.GetValues("X-Correlation-ID").Single());
        Assert.Equal("existing-channel", inner.LastRequest!.Headers.GetValues("X-Channel").Single());
        Assert.Equal("existing-agent", inner.LastRequest!.Headers.GetValues("X-Agent").Single());
    }

    [Fact]
    public async Task Adds_traceparent_when_missing()
    {
        var context = new TestObservabilityContext();
        var inner = new TestMessageHandler();
        var handler = new OutboundTraceContextHandler(context) { InnerHandler = inner };
        var client = new HttpClient(handler);

        await client.GetAsync("http://example.com");

        Assert.True(inner.LastRequest!.Headers.Contains("traceparent"));
    }

    [Fact]
    public async Task Does_not_overwrite_existing_traceparent()
    {
        var context = new TestObservabilityContext();
        var inner = new TestMessageHandler();
        var handler = new OutboundTraceContextHandler(context) { InnerHandler = inner };
        var client = new HttpClient(handler);

        const string existing = "00-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-bbbbbbbbbbbbbbbb-01";
        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        request.Headers.Add("traceparent", existing);

        await client.SendAsync(request);

        Assert.Equal(existing, inner.LastRequest!.Headers.GetValues("traceparent").Single());
    }
}
