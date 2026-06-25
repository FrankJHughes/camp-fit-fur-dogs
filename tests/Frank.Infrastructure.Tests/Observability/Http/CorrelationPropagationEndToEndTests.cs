#nullable enable
using System.Net;
using System.Net.Http.Json;
using Frank.Infrastructure.Tests.Observability.Helpers;
using Xunit;

namespace Frank.Infrastructure.Tests.Observability.Http;

public class CorrelationPropagationEndToEndTests : ObservabilityTestBase
{
    private sealed record CorrelationResponse(string CorrelationId);

    [Fact]
    public async Task End_to_end_uses_traceparent_when_present()
    {
        const string traceparent =
            "00-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-bbbbbbbbbbbbbbbb-01";

        Client.DefaultRequestHeaders.Add("traceparent", traceparent);

        var response = await Client.GetAsync("/__test__/correlation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CorrelationResponse>();
        Assert.NotNull(payload);
        Assert.Equal("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", payload!.CorrelationId);
    }

    [Fact]
    public async Task End_to_end_uses_correlation_header_when_no_traceparent()
    {
        Client.DefaultRequestHeaders.Add("X-Correlation-ID", "end-to-end-corr");

        var response = await Client.GetAsync("/__test__/correlation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CorrelationResponse>();
        Assert.NotNull(payload);
        Assert.Equal("end-to-end-corr", payload!.CorrelationId);
    }

    [Fact]
    public async Task End_to_end_generates_correlation_when_no_headers()
    {
        var response = await Client.GetAsync("/__test__/correlation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CorrelationResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.CorrelationId));
    }
}
