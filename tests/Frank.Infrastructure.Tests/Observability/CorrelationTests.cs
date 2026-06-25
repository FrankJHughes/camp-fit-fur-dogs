using System.Net;
using Frank.Infrastructure.Tests.Observability.Helpers;
using Xunit;

namespace Frank.Infrastructure.Tests.Observability;

public class CorrelationTests : ObservabilityTestBase
{
    [Fact]
    public async Task Adds_CorrelationId_When_Missing()
    {
        var response = await Client.GetAsync("/__test__/correlation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Correlation-ID"));

        var value = response.Headers.GetValues("X-Correlation-ID").Single();
        Assert.False(string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public async Task Propagates_CorrelationId_When_Provided()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/__test__/correlation");
        request.Headers.Add("X-Correlation-ID", "abc-123");

        var response = await Client.SendAsync(request);

        Assert.Equal("abc-123", response.Headers.GetValues("X-Correlation-ID").Single());
    }
}
