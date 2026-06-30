#nullable enable
using Frank.Abstractions.Observations;
using Frank.TestUtilities.Contexts;
using Frank.TestUtilities.Factories;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Infrastructure.Tests.Observability.Helpers;

public abstract class ObservabilityTestBase : IDisposable, IAsyncDisposable
{
    protected readonly ApiContext Context;
    protected readonly ApiFactory Factory;
    protected readonly HttpClient Client;

    protected ObservabilityTestBase()
    {
        Context = new ApiContext()
            .WithFake<IObservationSink>(new FakeObservabilitySink())
            .WithFake<IMetrics>(new FakeMetrics())
            .WithFake<IErrorBoundaryObserver>(new FakeErrorBoundaryObserver())
            .WithEndpointAssembly(typeof(Frank.TestUtilities.AssemblyMarker).Assembly);

        Factory = new ApiFactory(Context);
        Client = Factory.CreateClient(new ApiClientContext());
    }

    protected FakeObservabilitySink Trace => (FakeObservabilitySink)Context.GetFake<IObservationSink>();
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
