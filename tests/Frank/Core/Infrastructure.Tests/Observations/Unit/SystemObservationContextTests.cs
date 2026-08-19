using Frank.Core.Infrastructure.Clock;
using Frank.Core.Infrastructure.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class SystemObservationContextTests
{
    [Fact]
    public void Create_Produces_Valid_Context()
    {
        var env = new FakeEnv("Staging");
        var clock = new SystemClock();

        var ctx = SystemObservationContext.Create("jobs", "scheduler", env, clock);

        Assert.Equal("jobs", ctx.Channel);
        Assert.Equal("scheduler", ctx.Agent);
        Assert.Equal("Staging", ctx.Environment);
        Assert.NotEqual(default, ctx.Timestamp);
        Assert.Equal(32, ctx.CorrelationId.Length);
    }

    private sealed class FakeEnv : IHostEnvironment
    {
        public FakeEnv(string env) => EnvironmentName = env;

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "";
        public string ContentRootPath { get; set; } = "";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
            = new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
