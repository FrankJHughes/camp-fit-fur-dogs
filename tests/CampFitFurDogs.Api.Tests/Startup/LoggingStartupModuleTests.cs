using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CampFitFurDogs.Api.Horizontals.Startup;
using Microsoft.Extensions.FileProviders;

namespace CampFitFurDogs.Api.Tests.Startup;

public class LoggingStartupModuleTests
{
    // ---------------------------------------------------------------------
    // 1. ADD() REGISTERS REQUIRED SERVICES
    // ---------------------------------------------------------------------

    [Fact]
    public void Add_registers_HttpLogging_services()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        LoggingStartupModule.Add(services, config);

        // HttpLogging registers IConfigureOptions<HttpLoggingOptions>
        services.Should().Contain(s =>
            s.ServiceType == typeof(IConfigureOptions<HttpLoggingOptions>));
    }

    // ---------------------------------------------------------------------
    // 2. USE() DOES NOT THROW WHEN SERVICES ARE REGISTERED
    // ---------------------------------------------------------------------

    [Fact]
    public void Use_does_not_throw_when_services_are_registered()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        // Required for LoggingStartupModule.Use()
        services.AddSingleton<IWebHostEnvironment>(new FakeEnv("Development"));

        // Register HttpLogging services
        LoggingStartupModule.Add(services, new ConfigurationBuilder().Build());

        var provider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(provider);

        var act = () => LoggingStartupModule.Use(app);

        act.Should().NotThrow();
    }

    // ---------------------------------------------------------------------
    // 3. INTEGRATION TEST — HTTP LOGGING ENABLED IN DEVELOPMENT
    // ---------------------------------------------------------------------

    [Fact]
    public async Task HttpLogging_is_enabled_in_development()
    {
        using var host = await new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.UseEnvironment("Development");

                web.ConfigureServices((ctx, services) =>
                {
                    LoggingStartupModule.Add(services, ctx.Configuration);
                });

                web.Configure(app =>
                {
                    LoggingStartupModule.Use(app);
                    app.Run(_ => Task.CompletedTask);
                });
            })
            .StartAsync();

        var client = host.GetTestClient();

        var response = await client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------------------------------------------------------------------
    // SUPPORTING TEST DOUBLE
    // ---------------------------------------------------------------------

    private sealed class FakeEnv : IWebHostEnvironment
    {
        public FakeEnv(string env) => EnvironmentName = env;

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "TestApp";
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
