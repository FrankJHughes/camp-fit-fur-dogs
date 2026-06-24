#nullable enable
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Frank.Infrastructure.Tests.Observability.Http;

public class InboundTraceContextMiddlewareTests : ObservabilityPipelineTestBase
{
    private sealed record CorrelationResponse(string CorrelationId);

    [Fact]
    public async Task Uses_traceparent_trace_id_when_present()
    {
        // Arrange
        const string traceparent =
            "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";

        Client.DefaultRequestHeaders.Add("traceparent", traceparent);

        // Act
        var response = await Client.GetAsync("/__test__/correlation");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CorrelationResponse>();
        Assert.NotNull(payload);
        Assert.Equal("4bf92f3577b34da6a3ce929d0e0e4736", payload!.CorrelationId);
    }

    [Fact]
    public async Task Uses_correlation_header_when_no_traceparent()
    {
        // Arrange
        Client.DefaultRequestHeaders.Add("X-Correlation-ID", "abc-123");

        // Act
        var response = await Client.GetAsync("/__test__/correlation");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CorrelationResponse>();
        Assert.NotNull(payload);
        Assert.Equal("abc-123", payload!.CorrelationId);
    }

    [Fact]
    public async Task Falls_back_to_generated_correlation_when_no_headers()
    {
        // Act
        var response = await Client.GetAsync("/__test__/correlation");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CorrelationResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.CorrelationId));
    }
}
