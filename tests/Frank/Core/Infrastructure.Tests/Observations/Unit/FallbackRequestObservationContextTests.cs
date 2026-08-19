using Frank.Core.Infrastructure.Clock;
using Frank.Core.Infrastructure.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class FallbackRequestObservationContextTests
{
    [Fact]
    public void Constructor_Sets_Environment_And_Timestamp()
    {
        var env = new FakeEnv("Development");
        var clock = new SystemClock();

        var ctx = new FallbackRequestObservationContext(env, clock);

        Assert.Equal("Development", ctx.Environment);
        Assert.NotEqual(default, ctx.Timestamp);
        Assert.Equal("none", ctx.Channel);
        Assert.Equal("none", ctx.Agent);
        Assert.Null(ctx.UserId);
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
