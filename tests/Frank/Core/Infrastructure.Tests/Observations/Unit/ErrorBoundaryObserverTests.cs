using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Infrastructure.Observations;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class ErrorBoundaryObserverTests
{
    [Fact]
    public void OnError_Emits_Event_Without_Throwing()
    {
        var sink = new TestSink();
        var observer = new ErrorBoundaryObserver(sink);

        var ctx = new TestRequestObservationContext();
        var ex = new InvalidOperationException("boom");

        var thrown = Record.Exception(() => observer.OnError(ex, ctx));

        Assert.Null(thrown);
        Assert.Single(sink.Events);
        Assert.Equal("request.error", sink.Events[0].EventName);
    }

    private sealed class TestSink : IObservationSink
    {
        public List<(string EventName, string Category, string Severity, object? Payload)> Events { get; } = new();

        public void Emit(string eventName, string category, string severity, object? payload, IObservationContext context)
        {
            Events.Add((eventName, category, severity, payload));
        }
    }

    private sealed class TestRequestObservationContext : IRequestObservationContext
    {
        public string CorrelationId => "corr";
        public string Channel => "test";
        public string Agent => "agent";
        public string Environment => "env";
        public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
        public IReadOnlyDictionary<string, object?> Metadata => new Dictionary<string, object?>();
        public string? UserId => null;
        public void AddMetadata(string key, object? value) { }
    }
}
