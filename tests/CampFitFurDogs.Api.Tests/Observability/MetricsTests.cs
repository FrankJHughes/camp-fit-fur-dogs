using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes.Observability;
using Frank.Abstractions.Observability;

namespace CampFitFurDogs.Api.Tests.Observability;

public class MetricsTests
{
    [Fact]
    public async Task RecordsMetrics_OnSuccessfulRequest()
    {
        var ctx = new ApiContext()
            .WithFake<IMetrics>(new FakeMetrics());

        await using var api = new ApiFactory(ctx);
        var client = api.CreateClient(new ApiClientContext());

        await client.GetAsync("/__test__/health");

        var metrics = (FakeMetrics)ctx.GetFake<IMetrics>();

        Assert.True(metrics.Increments.Count + metrics.Gauges.Count + metrics.Timers.Count > 0);
    }

    [Fact]
    public async Task RecordsMetrics_OnErrorRequest()
    {
        var ctx = new ApiContext()
            .WithFake<IMetrics>(new FakeMetrics());

        await using var api = new ApiFactory(ctx);
        var client = api.CreateClient(new ApiClientContext());

        await client.GetAsync("/__test__/throw");

        var metrics = (FakeMetrics)ctx.GetFake<IMetrics>();

        Assert.True(metrics.Increments.Count + metrics.Gauges.Count + metrics.Timers.Count > 0);
    }
}
