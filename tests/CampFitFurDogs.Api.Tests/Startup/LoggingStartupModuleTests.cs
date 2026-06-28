using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using CampFitFurDogs.Api.Horizontals.Startup.Modules;

namespace CampFitFurDogs.Api.Tests.Startup;

public class LoggingStartupModuleTests
{
    // ------------------------------------------------------------
    // ADD() REGISTRATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Add_registers_HttpLogging_services()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Development"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<LoggingStartupModule>(config);

        var options = app.Services.GetService<IConfigureOptions<HttpLoggingOptions>>();

        options.Should().NotBeNull();
    }

    // ------------------------------------------------------------
    // USE() EXECUTION
    // ------------------------------------------------------------
    [Fact]
    public async Task Use_does_not_throw()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Development"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<LoggingStartupModule>(config);

        var module = new LoggingStartupModule();

        Action act = () => module.Use(app);

        act.Should().NotThrow();
    }

    // ------------------------------------------------------------
    // PIPELINE VALIDATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Pipeline_runs_successfully()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Development"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<LoggingStartupModule>(
            config,
            app => app.MapGet("/", () => "ok")
        );

        var client = app.GetTestClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
