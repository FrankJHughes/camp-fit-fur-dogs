using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes.Observability;
using Frank.Abstractions.Observability;

namespace CampFitFurDogs.Api.Tests.Observability;

public class TraceEventTests
{
    [Fact]
    public async Task EmitsTraceEvents_OnSuccessfulRequest()
    {
        var ctx = new ApiContext()
            .WithFake<IObservabilitySink>(new FakeObservabilitySink());

        await using var api = new ApiFactory(ctx);
        var client = api.CreateClient(new ApiClientContext());

        await client.GetAsync("/__test__/health");

        var trace = (FakeObservabilitySink)ctx.GetFake<IObservabilitySink>();

        Assert.NotEmpty(trace.Events);
        Assert.Contains(trace.Events, e => e.EventName != null);
    }

    [Fact]
    public async Task EmitsTraceEvents_OnErrorRequest()
    {
        var ctx = new ApiContext()
            .WithFake<IObservabilitySink>(new FakeObservabilitySink());

        await using var api = new ApiFactory(ctx);
        var client = api.CreateClient(new ApiClientContext());

        await client.GetAsync("/__test__/throw");

        var trace = (FakeObservabilitySink)ctx.GetFake<IObservabilitySink>();

        Assert.NotEmpty(trace.Events);

        // Your actual severity is lowercase "error"
        Assert.Contains(trace.Events, e =>
            e.Severity.Equals("error", StringComparison.OrdinalIgnoreCase));
    }
}
