using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Api.Horizontals.Startup.Modules;

namespace CampFitFurDogs.Api.Tests.Startup;

public class CorsStartupModuleTests
{
    private const string FrontendConfigKey = "Frontend__BaseUrl";

    // ------------------------------------------------------------
    // ORIGIN CONFIGURATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Cors_policy_uses_frontend_base_url_from_configuration()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production",
            [FrontendConfigKey] = "https://example.com/"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        var provider = app.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy.Should().NotBeNull();
        policy!.Origins.Should().ContainSingle()
            .Which.Should().Be("https://example.com");
    }

    [Fact]
    public async Task Development_defaults_to_localhost_3000()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Development"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        var provider = app.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy!.Origins.Should().ContainSingle()
            .Which.Should().Be("http://localhost:3000");
    }

    // ------------------------------------------------------------
    // ERROR CONDITIONS
    // ------------------------------------------------------------
    [Fact]
    public void Missing_frontend_base_url_in_production_throws()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production"
        };

        Func<Task> act = () => StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Frontend__BaseUrl*");
    }

    [Fact]
    public void Invalid_frontend_base_url_throws()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production",
            [FrontendConfigKey] = "not-a-valid-uri"
        };

        Func<Task> act = () => StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*valid absolute URI*");
    }

    [Fact]
    public void Wildcard_frontend_base_url_throws()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production",
            [FrontendConfigKey] = "http://*.example.com"
        };

        Func<Task> act = () => StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*must not contain wildcard*");
    }

    // ------------------------------------------------------------
    // POLICY CONTENT
    // ------------------------------------------------------------
    [Fact]
    public async Task Cors_policy_allows_expected_methods_and_headers()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production",
            [FrontendConfigKey] = "https://example.com/"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        var provider = app.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy!.Methods.Should().BeEquivalentTo("GET", "POST", "PUT", "DELETE");
        policy.Headers.Should().BeEquivalentTo("Authorization", "Content-Type");
        policy.SupportsCredentials.Should().BeTrue();
    }
}
