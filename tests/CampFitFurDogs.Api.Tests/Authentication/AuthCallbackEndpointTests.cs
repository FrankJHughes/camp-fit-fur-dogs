using System.Net;
using System.Net.Http;
using System.Text.Json;
using CampFitFurDogs.Application.Abstractions.Identity.External;
using CampFitFurDogs.Api.Tests.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Authentication;

public class AuthCallbackEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthCallbackEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private static readonly string First = "Frank";
    private static readonly string Last = "Smith";
    private static readonly string Email = "frank@example.com";

    // ------------------------------------------------------------
    // SUCCESS PATH
    // ------------------------------------------------------------
    [Fact]
    public async Task Successful_callback_sets_cookie_and_redirects()
    {
        var externalId = "auth0|abc123";
        var mappedCustomerId = Guid.NewGuid();

        var tokenResponseJson = JsonSerializer.Serialize(new { access_token = "fake_access_token" });

        var userInfoJson = JsonSerializer.Serialize(new
        {
            sub = externalId,
            given_name = First,
            family_name = Last,
            email = Email
        });

        var fakeHttp = new FakeHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userInfoJson)
            }
        });

        var resolver = new FakeExternalIdentityResolver(mappedCustomerId);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "dev-fake.auth0.com",
                    ["Auth0:ClientId"] = "client",
                    ["Auth0:ClientSecret"] = "secret",
                    ["Auth0:CallbackUrl"] = "http://localhost/api/auth/callback",
                    ["Auth0:PostLoginRedirectUrl"] = "http://localhost:3000/"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IExternalIdentityResolver>(resolver);
                services.AddSingleton(new HttpClient(fakeHttp));
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("http://localhost:3000/", response.Headers.Location!.ToString());

        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
        var cookie = cookies.First();
        Assert.Contains("cfd.session=", cookie);
        Assert.Contains(mappedCustomerId.ToString(), cookie);

        Assert.Equal(externalId, resolver.LastExternalId);
        Assert.Equal(First, resolver.LastFirstName);
        Assert.Equal(Last, resolver.LastLastName);
        Assert.Equal(Email, resolver.LastEmail);
    }

    // ------------------------------------------------------------
    // MISSING AUTHORIZATION CODE
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_authorization_code_returns_bad_request_problem_details()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "dev-fake.auth0.com",
                    ["Auth0:ClientId"] = "client",
                    ["Auth0:ClientSecret"] = "secret",
                    ["Auth0:CallbackUrl"] = "http://localhost/api/auth/callback",
                    ["Auth0:PostLoginRedirectUrl"] = "http://localhost:3000/"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(new HttpClient(new FakeHttpMessageHandler([])));
                services.AddSingleton<IExternalIdentityResolver>(new FakeExternalIdentityResolver(Guid.NewGuid()));
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Missing authorization code", json);
    }

    // ------------------------------------------------------------
    // INCOMPLETE CONFIGURATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Incomplete_configuration_returns_internal_server_error()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "",
                    ["Auth0:ClientId"] = "",
                    ["Auth0:ClientSecret"] = "",
                    ["Auth0:CallbackUrl"] = "",
                    ["Auth0:PostLoginRedirectUrl"] = ""
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(new HttpClient(new FakeHttpMessageHandler([])));
                services.AddSingleton<IExternalIdentityResolver>(new FakeExternalIdentityResolver(Guid.NewGuid()));
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Auth0 configuration is incomplete", json);
    }

    // ------------------------------------------------------------
    // MISSING ACCESS TOKEN
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_access_token_returns_bad_gateway_problem_details()
    {
        var tokenResponseJson = JsonSerializer.Serialize(new { access_token = "" });

        var fakeHttp = new FakeHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            }
        });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "dev-fake.auth0.com",
                    ["Auth0:ClientId"] = "client",
                    ["Auth0:ClientSecret"] = "secret",
                    ["Auth0:CallbackUrl"] = "http://localhost/api/auth/callback",
                    ["Auth0:PostLoginRedirectUrl"] = "http://localhost:3000/"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(new HttpClient(fakeHttp));
                services.AddSingleton<IExternalIdentityResolver>(new FakeExternalIdentityResolver(Guid.NewGuid()));
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Missing access token", json);
    }

    // ------------------------------------------------------------
    // USERINFO FAILURE
    // ------------------------------------------------------------
    [Fact]
    public async Task Userinfo_failure_returns_bad_gateway_problem_details()
    {
        var tokenResponseJson = JsonSerializer.Serialize(new { access_token = "fake_access_token" });

        var fakeHttp = new FakeHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "dev-fake.auth0.com",
                    ["Auth0:ClientId"] = "client",
                    ["Auth0:ClientSecret"] = "secret",
                    ["Auth0:CallbackUrl"] = "http://localhost/api/auth/callback",
                    ["Auth0:PostLoginRedirectUrl"] = "http://localhost:3000/"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(new HttpClient(fakeHttp));
                services.AddSingleton<IExternalIdentityResolver>(new FakeExternalIdentityResolver(Guid.NewGuid()));
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Failed to retrieve user profile", json);
    }

    // ------------------------------------------------------------
    // MISSING SUB CLAIM
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_sub_claim_returns_bad_gateway_problem_details()
    {
        var tokenResponseJson = JsonSerializer.Serialize(new { access_token = "fake_access_token" });

        var userInfoJson = JsonSerializer.Serialize(new
        {
            sub = "",
            given_name = First,
            family_name = Last,
            email = Email
        });

        var fakeHttp = new FakeHttpMessageHandler(new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(userInfoJson)
            }
        });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth0:Domain"] = "dev-fake.auth0.com",
                    ["Auth0:ClientId"] = "client",
                    ["Auth0:ClientSecret"] = "secret",
                    ["Auth0:CallbackUrl"] = "http://localhost/api/auth/callback",
                    ["Auth0:PostLoginRedirectUrl"] = "http://localhost:3000/"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(new HttpClient(fakeHttp));
                services.AddSingleton<IExternalIdentityResolver>(new FakeExternalIdentityResolver(Guid.NewGuid()));
            });
        })
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Missing Auth0 user identifier", json);
    }
}
