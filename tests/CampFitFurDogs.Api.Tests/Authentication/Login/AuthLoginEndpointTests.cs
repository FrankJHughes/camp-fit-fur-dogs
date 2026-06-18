using System.Net;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Authentication.Login;

public class AuthLoginEndpointTests : IAsyncLifetime
{
    private ApiFactory _api = default!;

    // ------------------------------------------------------------
    // TEST INITIALIZATION
    // ------------------------------------------------------------
    public Task InitializeAsync()
    {
        var ctx = new ApiContext()
            .WithDatabase(false) // Login endpoint does not use DB
            .WithCookieAuthOnly(false);

        _api = new ApiFactory(ctx);

        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // Helper: create client with config overrides
    private HttpClient CreateClientWithOverrides(Action<IConfigurationBuilder> apply)
    {
        var ctx = new ApiContext()
            .WithDatabase(false)
            .WithCookieAuthOnly(false)
            .WithConfigOverride(apply);

        var api = new ApiFactory(ctx);
        return api.CreateClient(new ApiClientContext());
    }

    // ------------------------------------------------------------
    // SUCCESS PATH
    // ------------------------------------------------------------
    [Fact]
    public async Task Login_redirects_to_auth0_authorize_url()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Callback:Oidc:Authority"] = "https://dev-fake.auth0.com",
                ["Authentication:Callback:Oidc:ClientId"] = "client123",
                ["Authentication:Callback:Oidc:ClientSecret"] = "secret123",
                ["Authentication:Callback:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
                ["Authentication:Callback:PostLoginRedirectUrl"] = "http://localhost:5173/",
                ["Authentication:Callback:Oidc:Disabled"] = "false",
                ["Frontend:BaseUrl"] = "/"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        var location = response.Headers.Location!.ToString();
        location.Should().StartWith("https://dev-fake.auth0.com/authorize?");

        var uri = new Uri(location);
        var query = QueryHelpers.ParseQuery(uri.Query);

        query["scope"].ToString().Should().Be("openid profile email");
        query["response_type"].ToString().Should().Be("code");
        query["client_id"].ToString().Should().Be("client123");
        query["redirect_uri"].ToString().Should().Be("http://localhost/api/auth/callback");
    }

    // ------------------------------------------------------------
    // ERROR PATHS
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_configuration_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Callback:Oidc:Authority"] = "",
                ["Authentication:Callback:Oidc:ClientId"] = "",
                ["Authentication:Callback:Oidc:CallbackUrl"] = "",
                ["Frontend:BaseUrl"] = "/"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Authentication configuration is missing or incomplete");
    }

    [Fact]
    public async Task Missing_domain_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Callback:Oidc:Authority"] = "",
                ["Authentication:Callback:Oidc:ClientId"] = "client123",
                ["Authentication:Callback:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
                ["Frontend:BaseUrl"] = "/"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Authentication configuration is missing or incomplete");
    }

    [Fact]
    public async Task Missing_client_id_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Callback:Oidc:Authority"] = "https://dev-fake.auth0.com",
                ["Authentication:Callback:Oidc:ClientId"] = "",
                ["Authentication:Callback:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
                ["Frontend:BaseUrl"] = "/"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Authentication configuration is missing or incomplete");
    }

    [Fact]
    public async Task Missing_callback_url_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Callback:Oidc:Authority"] = "https://dev-fake.auth0.com",
                ["Authentication:Callback:Oidc:ClientId"] = "client123",
                ["Authentication:Callback:Oidc:CallbackUrl"] = "",
                ["Frontend:BaseUrl"] = "/"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Authentication configuration is missing or incomplete");
    }
}
