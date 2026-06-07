using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CampFitFurDogs.Api.Horizontals.Startup;

namespace CampFitFurDogs.Api.Tests.Startup;

public class CorsStartupModuleTests
{
    private const string FrontendConfigKey = "Frontend__BaseUrl";
    private const string LocalFrontend = "http://localhost:3000";

    // ---------------------------------------------------------------------
    // 1. ORIGIN CONFIGURATION
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Cors_policy_uses_frontend_base_url_from_configuration()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "https://camp-fit-fur-dogs.vercel.app/"
        };

        using var host = await BuildHostAsync(config, "Production");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy.Should().NotBeNull();
        policy!.Origins.Should().ContainSingle()
            .Which.Should().Be("https://camp-fit-fur-dogs.vercel.app");
    }

    [Fact]
    public async Task Development_environment_defaults_to_local_frontend_when_not_configured()
    {
        var config = new Dictionary<string, string?>();

        using var host = await BuildHostAsync(config, "Development");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy.Should().NotBeNull();
        policy!.Origins.Should().ContainSingle()
            .Which.Should().Be(LocalFrontend);
    }

    [Fact]
    public void Missing_frontend_base_url_in_production_throws()
    {
        var config = new Dictionary<string, string?>();

        Func<Task> act = async () => await BuildHostAsync(config, "Production");

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Frontend__BaseUrl*");
    }

    [Fact]
    public void Invalid_frontend_base_url_throws()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "not-a-valid-uri"
        };

        Func<Task> act = async () => await BuildHostAsync(config, "Production");

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*valid absolute URI*");
    }

    [Fact]
    public void Wildcard_frontend_base_url_throws()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "http://*.example.com"
        };

        Func<Task> act = async () => await BuildHostAsync(config, "Production");

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*must not contain wildcard*");
    }

    // ---------------------------------------------------------------------
    // 2. ALLOWED METHODS + HEADERS
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Cors_policy_allows_expected_methods_and_headers()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "https://camp-fit-fur-dogs.vercel.app/"
        };

        using var host = await BuildHostAsync(config, "Production");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy.Should().NotBeNull();
        policy!.Methods.Should().BeEquivalentTo("GET", "POST", "PUT", "DELETE");
        policy.Headers.Should().BeEquivalentTo("Authorization", "Content-Type");
        policy.SupportsCredentials.Should().BeTrue();
    }

    [Fact]
    public async Task Cors_policy_rejects_disallowed_methods()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "https://camp-fit-fur-dogs.vercel.app/"
        };

        using var host = await BuildHostAsync(config, "Production");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy!.Methods.Should().NotContain("PATCH");
        policy.Methods.Should().NotContain("HEAD");
        policy.Methods.Should().NotContain("TRACE");
    }

    [Fact]
    public async Task Cors_policy_rejects_disallowed_headers()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "https://camp-fit-fur-dogs.vercel.app/"
        };

        using var host = await BuildHostAsync(config, "Production");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy!.Headers.Should().NotContain("X-Custom");
        policy.Headers.Should().NotContain("Accept-Language");
        policy.Headers.Should().NotContain("User-Agent");
    }

    // ---------------------------------------------------------------------
    // 3. NO WILDCARDS
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Cors_policy_does_not_use_wildcards()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "https://camp-fit-fur-dogs.vercel.app/"
        };

        using var host = await BuildHostAsync(config, "Production");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy.Should().NotBeNull();
        policy!.Origins.Should().AllSatisfy(o => o.Should().NotContain("*"));
        policy.Headers.Should().AllSatisfy(h => h.Should().NotBe("*"));
        policy.Methods.Should().AllSatisfy(m => m.Should().NotBe("*"));
    }

    // ---------------------------------------------------------------------
    // 4. PREFLIGHT BEHAVIOR
    // ---------------------------------------------------------------------

    [Fact]
    public async Task Preflight_request_includes_max_age_header()
    {
        var config = new Dictionary<string, string?>
        {
            [FrontendConfigKey] = "https://camp-fit-fur-dogs.vercel.app/"
        };

        using var host = await BuildHostAsync(config, "Production");

        var provider = host.Services.GetRequiredService<ICorsPolicyProvider>();
        var policy = await provider.GetPolicyAsync(new DefaultHttpContext(), null);

        policy!.PreflightMaxAge.Should().NotBeNull();
        policy.PreflightMaxAge!.Value.TotalSeconds.Should().BeGreaterThan(0);
    }

    // ---------------------------------------------------------------------
    // INTERNAL HOST BUILDER — now uses CorsStartupModule
    // ---------------------------------------------------------------------

    private static async Task<IHost> BuildHostAsync(
        IDictionary<string, string?> configValues,
        string environment)
    {
        var builder = new HostBuilder()
            .ConfigureHostConfiguration(cfg =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ASPNETCORE_ENVIRONMENT"] = environment
                });
            })
            .ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(configValues);
            })
            .ConfigureWebHost(webHost =>
            {
                webHost
                    .UseTestServer()
                    .ConfigureServices((ctx, services) =>
                    {
                        CorsStartupModule.Add(services, ctx.Configuration);
                    })
                    .Configure(app => { });
            });

        return await builder.StartAsync();
    }
}
