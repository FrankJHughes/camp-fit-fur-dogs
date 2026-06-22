using System.Net;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;

namespace CampFitFurDogs.Api.Tests.Observability;

public class CorrelationTests
{
    [Fact]
    public async Task AddsCorrelationId_WhenMissing()
    {
        await using var api = new ApiFactory(new ApiContext());
        var client = api.CreateClient(new ApiClientContext());

        var response = await client.GetAsync("/__test__/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Correlation-ID"));

        var value = response.Headers.GetValues("X-Correlation-ID").Single();
        Assert.False(string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public async Task PropagatesCorrelationId_WhenProvided()
    {
        await using var api = new ApiFactory(new ApiContext());
        var client = api.CreateClient(new ApiClientContext());

        var request = new HttpRequestMessage(HttpMethod.Get, "/__test__/health");
        request.Headers.Add("X-Correlation-ID", "abc-123");

        var response = await client.SendAsync(request);

        var value = response.Headers.GetValues("X-Correlation-ID").Single();
        Assert.Equal("abc-123", value);
    }
}
