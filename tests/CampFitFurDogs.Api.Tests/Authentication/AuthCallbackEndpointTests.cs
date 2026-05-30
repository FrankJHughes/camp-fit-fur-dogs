using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Infrastructure.Identity.Oidc;
using CampFitFurDogs.TestUtilities.Fakes;

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

    private static Dictionary<string, string?> ValidOidcConfig => new()
    {
        ["Authentication:Oidc:Authority"] = "dev-fake.auth0.com",
        ["Authentication:Oidc:ClientId"] = "client",
        ["Authentication:Oidc:ClientSecret"] = "secret",
        ["Authentication:Oidc:CallbackUrl"] = "http://localhost/api/auth/callback",
        ["Authentication:Oidc:PostLoginRedirectUrl"] = "http://localhost:3000/"
    };

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

        var fakeHttp = new FakeHttpMessageHandler(new()
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(userInfoJson)
            }
        });

        var resolver = new FakeIdentityResolver { Result = mappedCustomerId };

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(resolver);
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Be("http://localhost:3000/");

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookie = cookies!.First();

        cookie.Should().Contain("cfd.session=");
        cookie.Should().Contain(mappedCustomerId.ToString());

        resolver.LastExternalId.Should().Be(externalId);
        resolver.LastFirstName.Should().Be(First);
        resolver.LastLastName.Should().Be(Last);
        resolver.LastEmail.Should().Be(Email);
    }

    [Fact]
    public async Task Successful_callback_logs_audit_event()
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

        var fakeHttp = new FakeHttpMessageHandler(new()
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(userInfoJson)
            }
        });

        var resolver = new FakeIdentityResolver { Result = mappedCustomerId };
        var audit = new FakeAuditLogger();

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(resolver);
                services.AddSingleton<IAuditLogger>(audit);
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        await client.GetAsync("/api/auth/callback?code=abc123");

        // Updated assertions to match TestUtilities FakeAuditLogger
        audit.CapturedCustomerId.Should().Be(mappedCustomerId);
        audit.CapturedExternalId.Should().Be(externalId);
    }

    // ------------------------------------------------------------
    // MISSING AUTHORIZATION CODE
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_authorization_code_returns_bad_request_problem_details()
    {
        var fakeHttp = new FakeHttpMessageHandler(new());

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(
                    new FakeIdentityResolver { Result = Guid.NewGuid() });
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Missing authorization code");
    }

    // ------------------------------------------------------------
    // INCOMPLETE CONFIGURATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Incomplete_configuration_returns_internal_server_error()
    {
        var fakeHttp = new FakeHttpMessageHandler(new());

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Oidc:Authority"] = "",
                    ["Authentication:Oidc:ClientId"] = "",
                    ["Authentication:Oidc:ClientSecret"] = "",
                    ["Authentication:Oidc:CallbackUrl"] = "",
                    ["Authentication:Oidc:PostLoginRedirectUrl"] = ""
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(
                    new FakeIdentityResolver { Result = Guid.NewGuid() });
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Bad Configuration");
    }

    // ------------------------------------------------------------
    // MISSING ACCESS TOKEN
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_access_token_returns_bad_gateway_problem_details()
    {
        var tokenResponseJson = JsonSerializer.Serialize(new { access_token = "" });

        var fakeHttp = new FakeHttpMessageHandler(new()
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            }
        });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(
                    new FakeIdentityResolver { Result = Guid.NewGuid() });
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Missing access token");
    }

    // ------------------------------------------------------------
    // USERINFO FAILURE
    // ------------------------------------------------------------
    [Fact]
    public async Task Userinfo_failure_returns_bad_gateway_problem_details()
    {
        var tokenResponseJson = JsonSerializer.Serialize(new { access_token = "fake_access_token" });

        var fakeHttp = new FakeHttpMessageHandler(new()
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new(HttpStatusCode.InternalServerError)
        });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(
                    new FakeIdentityResolver { Result = Guid.NewGuid() });
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Failed to retrieve user profile");
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

        var fakeHttp = new FakeHttpMessageHandler(new()
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(userInfoJson)
            }
        });

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(
                    new FakeIdentityResolver { Result = Guid.NewGuid() });
            });
        })
        .CreateClient(new() { AllowAutoRedirect = false });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Missing Auth0 user identifier");
    }

    // ------------------------------------------------------------
    // COOKIE SECURITY IN PRODUCTION
    // ------------------------------------------------------------
    [Fact]
    public async Task Cookie_is_secure_in_production()
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

        var fakeHttp = new FakeHttpMessageHandler(new()
        {
            ["https://dev-fake.auth0.com/oauth/token"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(tokenResponseJson)
            },
            ["https://dev-fake.auth0.com/userinfo"] = new(HttpStatusCode.OK)
            {
                Content = new StringContent(userInfoJson)
            }
        });

        var resolver = new FakeIdentityResolver { Result = mappedCustomerId };

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");

            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(ValidOidcConfig);
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpClient<IAuthClient, OidcAuthClient>()
                        .ConfigurePrimaryHttpMessageHandler(() => fakeHttp);

                services.AddSingleton<IIdentityResolver>(resolver);
                services.AddSingleton<IAuditLogger>(new FakeAuditLogger());
            });
        })
        .CreateClient(new()
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var cookie = cookies!.First();

        cookie.ToLowerInvariant().Should().Contain("secure");
    }
}
