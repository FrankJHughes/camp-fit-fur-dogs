#nullable enable
using Frank.Abstractions.Observability;
using Frank.TestUtilities.Contexts;
using Frank.TestUtilities.Factories;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Infrastructure.Tests.Observability.Http;

public abstract class ObservabilityPipelineTestBase : IDisposable, IAsyncDisposable
{
    protected readonly ApiContext Context;
    protected readonly ApiFactory Factory;
    protected readonly HttpClient Client;

    protected ObservabilityPipelineTestBase()
    {
        Context = new ApiContext()
            .WithFake<ITraceEvents>(new FakeTraceEvents())
            .WithFake<IMetrics>(new FakeMetrics())
            .WithFake<IErrorBoundaryObserver>(new FakeErrorBoundaryObserver())
            .WithEndpointAssembly(typeof(Frank.TestUtilities.AssemblyMarker).Assembly);

        Factory = new ApiFactory(Context);
        Client = Factory.CreateClient(new ApiClientContext());
    }

    protected FakeTraceEvents Trace => Context.GetFake<FakeTraceEvents>();
    protected FakeMetrics Metrics => Context.GetFake<FakeMetrics>();
    protected FakeErrorBoundaryObserver Errors => Context.GetFake<FakeErrorBoundaryObserver>();

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
    }
}
