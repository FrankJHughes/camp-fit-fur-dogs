using Frank.Abstractions.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Startup;

public static class StartupModuleTestHostWebApp
{
    public static async Task<WebApplication> CreateAsync<TModule>(
        IDictionary<string, string?> configValues,
        Action<WebApplication>? configureApp = null)
        where TModule : IStartupModule, new()
    {
        if (!configValues.TryGetValue("ASPNETCORE_ENVIRONMENT", out var environment) ||
            string.IsNullOrWhiteSpace(environment))
        {
            throw new InvalidOperationException(
                "StartupModuleTestHostWebApp requires 'ASPNETCORE_ENVIRONMENT' to be provided.");
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = environment
        });

        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(configValues);

        var module = new TModule();

        // ADD PHASE
        module.Add(builder);

        var app = builder.Build();

        // Allow tests to add endpoints BEFORE StartAsync()
        configureApp?.Invoke(app);

        // USE PHASE
        module.Use(app);

        await app.StartAsync();

        return app;
    }
}
