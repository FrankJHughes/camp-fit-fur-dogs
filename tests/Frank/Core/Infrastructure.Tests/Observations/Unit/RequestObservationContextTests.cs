using Frank.Core.Infrastructure.Clock;
using Frank.Core.Infrastructure.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Tests.Observations.Unit;

public class RequestObservationContextTests
{
    [Fact]
    public void Create_Injects_UserId_Into_Metadata()
    {
        var env = new FakeEnv("Prod");
        var clock = new SystemClock();

        var ctx = RequestObservationContext.Create(
            userId: "user-123",
            correlationId: "abc",
            channel: "http",
            agent: "browser",
            environment: env,
            clock: clock);

        Assert.Equal("user-123", ctx.UserId);
        Assert.Equal("user-123", ctx.Metadata["user.id"]);
    }

    [Fact]
    public void Create_Does_Not_Inject_UserId_When_Null()
    {
        var env = new FakeEnv("Prod");
        var clock = new SystemClock();

        var ctx = RequestObservationContext.Create(
            userId: null,
            correlationId: "abc",
            channel: "http",
            agent: "browser",
            environment: env,
            clock: clock);

        Assert.False(ctx.Metadata.ContainsKey("user.id"));
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
