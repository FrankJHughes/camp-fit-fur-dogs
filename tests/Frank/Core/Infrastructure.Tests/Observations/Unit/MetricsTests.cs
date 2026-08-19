using Frank.Core.Infrastructure.Observations;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class MetricsTests
{
    [Fact]
    public void Increment_Does_Not_Throw()
    {
        var metrics = new Metrics();

        var ex = Record.Exception(() => metrics.Increment("requests"));

        Assert.Null(ex);
    }

    [Fact]
    public void Gauge_Does_Not_Throw()
    {
        var metrics = new Metrics();

        var ex = Record.Exception(() => metrics.Gauge("memory", 42.0));

        Assert.Null(ex);
    }

    [Fact]
    public void Timer_Dispose_Does_Not_Throw()
    {
        var metrics = new Metrics();

        using var timer = metrics.Timer("duration");

        // No exception expected
    }
}
