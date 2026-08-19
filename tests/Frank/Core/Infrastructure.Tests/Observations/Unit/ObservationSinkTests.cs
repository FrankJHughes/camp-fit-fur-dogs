using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Infrastructure.Observations;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class ObservationSinkTests
{
    [Fact]
    public void Emit_Does_Not_Throw()
    {
        var sink = new ObservationSink();
        var ctx = new TestContext();

        var ex = Record.Exception(() =>
            sink.Emit("event", "cat", "info", new { Value = 1 }, ctx));

        Assert.Null(ex);
    }

    private sealed class TestContext : IObservationContext
    {
        public string CorrelationId => "corr";
        public string Channel => "test";
        public string Agent => "agent";
        public string Environment => "env";
        public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
        public IReadOnlyDictionary<string, object?> Metadata => new Dictionary<string, object?>();
        public void AddMetadata(string key, object? value) { }
    }
}
