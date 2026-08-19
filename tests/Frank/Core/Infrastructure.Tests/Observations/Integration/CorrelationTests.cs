using System.Net;
using Frank.TestUtilities.Helpers;

namespace Frank.Core.Infrastructure.Tests.Observations.Integration;

public class CorrelationTests : ObservationsTestBase
{
    [Fact]
    public async Task Adds_CorrelationId_When_Missing()
    {
        var response = await Client.GetAsync("/api/__test__/correlation");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Correlation-ID"));

        var value = response.Headers.GetValues("X-Correlation-ID").Single();
        Assert.False(string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public async Task Propagates_CorrelationId_When_Provided()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/__test__/correlation");
        request.Headers.Add("X-Correlation-ID", "abc-123");

        var response = await Client.SendAsync(request);

        Assert.Equal("abc-123", response.Headers.GetValues("X-Correlation-ID").Single());
    }
}
