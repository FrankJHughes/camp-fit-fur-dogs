using System.Net;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Authentication;

[Collection("Auth Login Endpoint Tests")]
public class AuthLoginEndpointTests : ApiWithoutDatabaseTestBase
{
    public AuthLoginEndpointTests(CampFitFurDogsApiFactory factory)
        : base(factory)
    {
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
                ["Authentication:Oidc:Authority"] = "dev-fake.auth0.com",
                ["Authentication:Oidc:ClientId"] = "client123",
                ["Authentication:Oidc:ClientSecret"] = "secret123",
                ["Authentication:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
                ["Authentication:Oidc:PostLoginRedirectUrl"] = "http://localhost:5173/",
                ["Authentication:Oidc:Disabled"] = "false"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var location = response.Headers.Location!.ToString();
        Assert.StartsWith("https://dev-fake.auth0.com/authorize?", location);

        var uri = new Uri(location);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        Assert.Equal("openid profile email", query["scope"]);
        Assert.Equal("code", query["response_type"]);
        Assert.Equal("client123", query["client_id"]);
        Assert.Equal("http://localhost/api/auth/callback", query["redirect_uri"]);
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
                ["Authentication:Oidc:Authority"] = "",
                ["Authentication:Oidc:ClientId"] = "",
                ["Authentication:Oidc:CallbackUrl"] = ""
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Auth0 configuration is missing or incomplete", json);
    }

    [Fact]
    public async Task Missing_domain_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Oidc:Authority"] = "",
                ["Authentication:Oidc:ClientId"] = "client123",
                ["Authentication:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Auth0 configuration is missing or incomplete", json);
    }

    [Fact]
    public async Task Missing_client_id_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Oidc:Authority"] = "dev-fake.auth0.com",
                ["Authentication:Oidc:ClientId"] = "",
                ["Authentication:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback"
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Auth0 configuration is missing or incomplete", json);
    }

    [Fact]
    public async Task Missing_callback_url_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Oidc:Authority"] = "dev-fake.auth0.com",
                ["Authentication:Oidc:ClientId"] = "client123",
                ["Authentication:Oidc:CallbackUrl"] = ""
            });
        });

        var response = await client.GetAsync("/api/auth/login");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Auth0 configuration is missing or incomplete", json);
    }
}
