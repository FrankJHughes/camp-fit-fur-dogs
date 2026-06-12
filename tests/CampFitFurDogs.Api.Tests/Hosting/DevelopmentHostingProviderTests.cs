using CampFitFurDogs.Api.Horizontals.Hosting.Modules;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.Api.Tests.Hosting;

public sealed class DevelopmentHostingModuleTests : IDisposable
{
    private readonly string? _originalEnv;
    private readonly string? _originalPort;

    public DevelopmentHostingModuleTests()
    {
        _originalEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        _originalPort = Environment.GetEnvironmentVariable("PORT");
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _originalEnv);
        Environment.SetEnvironmentVariable("PORT", _originalPort);
    }

    // ------------------------------------------------------------
    // ACTIVATION LOGIC
    // ------------------------------------------------------------

    [Fact]
    public void IsActive_returns_true_when_environment_is_Development()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        var module = new LocalDevelopmentHostingModule();
        var builder = WebApplication.CreateBuilder();

        module.IsActive(builder).Should().BeTrue();
    }

    [Fact]
    public void IsActive_returns_false_when_environment_is_not_Development()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

        var module = new LocalDevelopmentHostingModule();
        var builder = WebApplication.CreateBuilder();

        module.IsActive(builder).Should().BeFalse();
    }

    // ------------------------------------------------------------
    // CONFIGURATION OVERRIDES
    // ------------------------------------------------------------

    [Fact]
    public async Task GetConfigurationOverridesAsync_sets_default_port_when_PORT_not_set()
    {
        Environment.SetEnvironmentVariable("PORT", null);

        var builder = WebApplication.CreateBuilder();
        var module = new LocalDevelopmentHostingModule();

        var overrides = await module.GetConfigurationOverridesAsync(builder);

        // Host-level setting
        builder.WebHost.GetSetting("urls")
            .Should().Be("http://0.0.0.0:8080");

        // Module returns frontend override
        overrides["Frontend__BaseUrl"]
            .Should().Be("https://localhost:5173");
    }

    [Fact]
    public async Task GetConfigurationOverridesAsync_uses_PORT_environment_variable_when_set()
    {
        Environment.SetEnvironmentVariable("PORT", "5555");

        var builder = WebApplication.CreateBuilder();
        var module = new LocalDevelopmentHostingModule();

        var overrides = await module.GetConfigurationOverridesAsync(builder);

        builder.WebHost.GetSetting("urls")
            .Should().Be("http://0.0.0.0:5555");

        overrides["Frontend__BaseUrl"]
            .Should().Be("https://localhost:5173");
    }

    // ------------------------------------------------------------
    // OVERRIDE CONTENT
    // ------------------------------------------------------------

    [Fact]
    public async Task Overrides_contain_expected_frontend_url()
    {
        var builder = WebApplication.CreateBuilder();
        var module = new LocalDevelopmentHostingModule();

        var overrides = await module.GetConfigurationOverridesAsync(builder);

        overrides.Should().ContainKey("Frontend__BaseUrl");
        overrides["Frontend__BaseUrl"].Should().Be("https://localhost:5173");
    }
}
