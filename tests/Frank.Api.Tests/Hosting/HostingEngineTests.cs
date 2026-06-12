using FluentAssertions;
using Frank.Api.Hosting;
using Frank.Abstractions.Hosting;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace Frank.Api.Tests.Hosting;

public sealed class HostingEngineTests
{
    // ------------------------------------------------------------
    // Fake module for testing
    // ------------------------------------------------------------
    private sealed class FakeModule : IHostingModule
    {
        public bool Active { get; init; }
        public bool Called { get; private set; }
        public Exception? ExceptionToThrow { get; init; }
        public IDictionary<string, string?> Overrides { get; init; } = new Dictionary<string, string?>();

        public bool IsActive(WebApplicationBuilder builder) => Active;

        public Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder)
        {
            Called = true;

            if (ExceptionToThrow is not null)
                throw ExceptionToThrow;

            return Task.FromResult(Overrides);
        }
    }

    // ------------------------------------------------------------
    // TESTS
    // ------------------------------------------------------------

    [Fact]
    public async Task Inactive_modules_are_skipped()
    {
        var builder = WebApplication.CreateBuilder();

        var m1 = new FakeModule { Active = false };
        var m2 = new FakeModule { Active = false };

        var engine = new HostingEngine(new[] { m1, m2 });

        await engine.ApplyHostingEnvironmentConfigurationAsync(builder);

        m1.Called.Should().BeFalse();
        m2.Called.Should().BeFalse();
    }

    [Fact]
    public async Task Active_modules_are_called()
    {
        var builder = WebApplication.CreateBuilder();

        var m1 = new FakeModule { Active = true };
        var m2 = new FakeModule { Active = true };

        var engine = new HostingEngine(new[] { m1, m2 });

        await engine.ApplyHostingEnvironmentConfigurationAsync(builder);

        m1.Called.Should().BeTrue();
        m2.Called.Should().BeTrue();
    }

    [Fact]
    public async Task Later_modules_override_earlier_values()
    {
        var builder = WebApplication.CreateBuilder();

        var m1 = new FakeModule
        {
            Active = true,
            Overrides = new Dictionary<string, string?>
            {
                ["Key"] = "Value1"
            }
        };

        var m2 = new FakeModule
        {
            Active = true,
            Overrides = new Dictionary<string, string?>
            {
                ["Key"] = "Value2"
            }
        };

        var engine = new HostingEngine(new[] { m1, m2 });

        await engine.ApplyHostingEnvironmentConfigurationAsync(builder);

        builder.Configuration["Key"].Should().Be("Value2");
    }

    [Fact]
    public async Task Exception_in_module_bubbles_up()
    {
        var builder = WebApplication.CreateBuilder();

        var failing = new FakeModule
        {
            Active = true,
            ExceptionToThrow = new InvalidOperationException("boom")
        };

        var engine = new HostingEngine(new[] { failing });

        Func<Task> act = () => engine.ApplyHostingEnvironmentConfigurationAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*boom*");
    }

    [Fact]
    public async Task No_active_modules_does_not_throw()
    {
        var builder = WebApplication.CreateBuilder();

        var m1 = new FakeModule { Active = false };
        var m2 = new FakeModule { Active = false };

        var engine = new HostingEngine(new[] { m1, m2 });

        await engine.ApplyHostingEnvironmentConfigurationAsync(builder);

        m1.Called.Should().BeFalse();
        m2.Called.Should().BeFalse();
    }
}
