using System.Net;
using System.Text.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Authentication;

public class AuthCallbackEndpointTests
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public AuthCallbackEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

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
        var mappedCustomerId = Guid.NewGuid();

        var fakeService = new FakeAuthCallbackService
        {
            CustomerId = CustomerId.From(mappedCustomerId),
            RedirectUrl = "http://localhost:3000/"
        };

        var client = new TestClientBuilder(_factory)
            .WithAuthCallbackService(fakeService)
            .BuildClient();

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

        var client = new TestClientBuilder(_factory)
            .WithAuthCallbackService(fakeService)
            .BuildClient();

        await client.GetAsync("/api/auth/callback?code=abc123");

        audit.CapturedCustomerId.Should().Be(mappedCustomerId);
    }

    // ------------------------------------------------------------
    // ERROR PATHS
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_authorization_code_returns_bad_request_problem_details()
    {
        var client = new TestClientBuilder(_factory).BuildClient();

        var response = await client.GetAsync("/api/auth/callback");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Missing authorization code");
    }

    [Fact]
    public async Task Incomplete_configuration_returns_internal_server_error()
    {
        var client = new TestClientBuilder(_factory)
            .WithConfigOverrides(cfg =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Oidc:Authority"] = "",
                    ["Authentication:Oidc:ClientId"] = "",
                    ["Authentication:Oidc:ClientSecret"] = "",
                    ["Authentication:Oidc:CallbackUrl"] = "",
                    ["Authentication:Oidc:PostLoginRedirectUrl"] = ""
                });
            })
            .BuildClient();

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Bad Configuration");
    }

    [Fact]
    public async Task Missing_access_token_returns_bad_gateway_problem_details()
    {
        var fakeResponses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] =
                new(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new { access_token = "" }))
                }
        };

        var client = new TestClientBuilder(_factory)
            .WithConfigOverrides(cfg => cfg.AddInMemoryCollection(ValidOidcConfig))
            .WithFakeOidcResponses(fakeResponses)
            .BuildClient();

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Missing access token");
    }

    [Fact]
    public async Task Userinfo_failure_returns_bad_gateway_problem_details()
    {
        var fakeResponses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] =
                new(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new { access_token = "fake_access_token" }))
                },

            ["https://dev-fake.auth0.com/userinfo"] =
                new(HttpStatusCode.InternalServerError)
        };

        var client = new TestClientBuilder(_factory)
            .WithConfigOverrides(cfg => cfg.AddInMemoryCollection(ValidOidcConfig))
            .WithFakeOidcResponses(fakeResponses)
            .BuildClient();

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Failed to retrieve user profile");
    }

    [Fact]
    public async Task Missing_sub_claim_returns_bad_gateway_problem_details()
    {
        var fakeResponses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://dev-fake.auth0.com/oauth/token"] =
                new(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new { access_token = "fake_access_token" }))
                },

            ["https://dev-fake.auth0.com/userinfo"] =
                new(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        sub = "",
                        given_name = "Frank",
                        family_name = "Smith",
                        email = "frank@example.com"
                    }))
                }
        };

        var client = new TestClientBuilder(_factory)
            .WithConfigOverrides(cfg => cfg.AddInMemoryCollection(ValidOidcConfig))
            .WithFakeOidcResponses(fakeResponses)
            .BuildClient();

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Missing Auth0 user identifier");
    }

    // ------------------------------------------------------------
    // COOKIE SECURITY
    // ------------------------------------------------------------
    [Fact]
    public async Task Cookie_is_secure_in_production()
    {
        var mappedCustomerId = Guid.NewGuid();

        var fakeService = new FakeAuthCallbackService
        {
            CustomerId = CustomerId.From(mappedCustomerId),
            RedirectUrl = "http://localhost:3000/"
        };

        var client = new TestClientBuilder(_factory)
            .WithEnvironment("Production")
            .WithAuthCallbackService(fakeService)
            .BuildClient();

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.First().ToLowerInvariant().Should().Contain("secure");
    }
}
