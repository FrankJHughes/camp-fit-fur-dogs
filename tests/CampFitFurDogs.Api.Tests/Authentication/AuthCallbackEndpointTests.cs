using System.Net;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Api.Tests.Authentication;

[Collection("Auth Callback Endpoint Tests")]
public class AuthCallbackEndpointTests : ApiWithPostgresTestBase
{
    public AuthCallbackEndpointTests(
        CampFitFurDogsApiFactory apiFactory,
        PostgresFixture postgresFixture)
        : base(apiFactory, postgresFixture)
    {
    }

    // ------------------------------------------------------------
    // SUCCESS PATH
    // ------------------------------------------------------------
    [Fact]
    public async Task Successful_callback_sets_cookie_and_redirects()
    {
        var mappedCustomerId = Guid.NewGuid();

        var fakeService = new FakeAuthCallbackService
        {
            CustomerId = CustomerId.From(mappedCustomerId),
            RedirectUrl = "http://localhost:3000/"
        };

        var client = CreateClientWithOverrides(
            overrides: null,
            options: null,
            configureServices: services =>
            {
                services.AddSingleton<IAuthCallbackService>(fakeService);
                services.AddSingleton<IAuthClient, FakeOidcAuthClient>();
            });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Be("http://localhost:3000/");

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.First().Should().Contain("cfd.session=");
    }

    [Fact]
    public async Task Successful_callback_logs_audit_event()
    {
        var mappedCustomerId = Guid.NewGuid();
        var audit = new FakeAuditLogger();

        var fakeService = new FakeAuthCallbackService
        {
            CustomerId = CustomerId.From(mappedCustomerId),
            RedirectUrl = "http://localhost:3000/",
            AuditLogger = audit
        };

        var client = CreateClientWithOverrides(
            overrides: null,
            options: null,
            configureServices: services =>
            {
                services.AddSingleton<IAuthCallbackService>(fakeService);
                services.AddSingleton<IAuthClient, FakeOidcAuthClient>();
            });

        await client.GetAsync("/api/auth/callback?code=abc123");

        audit.CapturedCustomerId.Should().Be(mappedCustomerId);
    }

    // ------------------------------------------------------------
    // ERROR PATHS
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_authorization_code_returns_bad_request_problem_details()
    {
        var client = CreateClientWithOverrides();

        var response = await client.GetAsync("/api/auth/callback");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Missing authorization code");
    }

    [Fact]
    public async Task Incomplete_configuration_returns_internal_server_error()
    {
        var client = CreateClientWithOverrides(cfg =>
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

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Bad Configuration");
    }

    [Fact]
    public async Task Missing_access_token_returns_bad_gateway_problem_details()
    {
        var client = CreateClientWithOverrides(
            overrides: null,
            options: null,
            configureServices: services =>
            {
                services.AddSingleton<IAuthClient, FakeOidcAuthClient_MissingAccessToken>();
            });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Missing access token");
    }

    [Fact]
    public async Task Userinfo_failure_returns_bad_gateway_problem_details()
    {
        var client = CreateClientWithOverrides(
            overrides: null,
            options: null,
            configureServices: services =>
            {
                services.AddSingleton<IAuthClient, FakeOidcAuthClient_UserInfoFailure>();
            });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Failed to retrieve user profile");
    }

    [Fact]
    public async Task Missing_sub_claim_returns_bad_gateway_problem_details()
    {
        var client = CreateClientWithOverrides(
            overrides: null,
            options: null,
            configureServices: services =>
            {
                services.AddSingleton<IAuthClient, FakeOidcAuthClient_MissingSub>();
            });

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().MatchRegex(".*(Missing Auth0 user identifier|MissingExternalId).*");
    }

    // ------------------------------------------------------------
    // COOKIE SECURITY
    // ------------------------------------------------------------
    [Fact]
    public void Cookie_is_secure_in_production()
    {
        var factory = Factory
            .Clone()
            .WithEnvironment("Production")
            .WithoutCookieHttpOverride()
            .UseContainer(PostgresFixture.Container);

        using var scope = factory.Services.CreateScope();
        var optionsMonitor = scope.ServiceProvider
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();

        var cookie = optionsMonitor.Get("cfd.session").Cookie;

        cookie.SecurePolicy.Should().Be(CookieSecurePolicy.Always,
            "production cookies must always require HTTPS");
    }
}
