using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Api.Horizontal.Startup.Modules;

namespace CampFitFurDogs.Api.Tests.Startup;

public class CorsStartupModuleTests
{
    private const string FrontendConfigKey = "Frontend:BaseUrl";
    private const string OidcConfigKey = "Authentication:Callback:Oidc:Authority";

    // ------------------------------------------------------------
    // ORIGIN CONFIGURATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Cors_policy_includes_frontend_base_url_and_oidc_authority_from_configuration()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production",
            [FrontendConfigKey] = "https://frontend.com",
            [OidcConfigKey] = "https://oidc.com"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        var provider = app.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy.Should().NotBeNull();
        policy!.Origins.Should().Contain("https://frontend.com");
        policy!.Origins.Should().Contain("https://oidc.com");
    }

    [Fact]
    public async Task Development_defaults_to_localhost_3000()
    {
        var config = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Development",
            [OidcConfigKey] = "https://oidc.com"
        };

        var app = await StartupModuleTestHostWebApp.CreateAsync<CorsStartupModule>(config);

        var provider = app.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy!.Origins.Should().Contain("http://localhost:3000");
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
            .WithMessage("*Frontend:BaseUrl*");
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

        policy!.Methods.Should().BeEquivalentTo("*");
        policy.Headers.Should().BeEquivalentTo("*");
        policy.SupportsCredentials.Should().BeTrue();
    }
}
