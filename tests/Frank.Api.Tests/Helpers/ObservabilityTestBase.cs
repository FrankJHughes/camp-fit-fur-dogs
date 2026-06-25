using Frank.Abstractions.Observability;
using Frank.TestUtilities.Contexts;
using Frank.TestUtilities.Factories;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Api.Tests.Helpers;

public abstract class ObservabilityTestBase : IDisposable, IAsyncDisposable
{
    protected readonly ApiContext Context;
    protected readonly ApiFactory Factory;
    protected readonly HttpClient Client;

    protected ObservabilityTestBase()
    {
        Context = new ApiContext()
            .WithFake<IObservabilitySink>(new FakeTraceEvents())
            .WithFake<IMetrics>(new FakeMetrics())
            .WithFake<IErrorBoundaryObserver>(new FakeErrorBoundaryObserver())
            .WithEndpointAssembly(typeof(Frank.TestUtilities.AssemblyMarker).Assembly);

        Factory = new ApiFactory(Context);
        Client = Factory.CreateClient(new ApiClientContext());
    }

    protected FakeTraceEvents Trace => (FakeTraceEvents)Context.GetFake<IObservabilitySink>();
    protected FakeMetrics Metrics => (FakeMetrics)Context.GetFake<IMetrics>();
    protected FakeErrorBoundaryObserver Errors => (FakeErrorBoundaryObserver)Context.GetFake<IErrorBoundaryObserver>();

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
