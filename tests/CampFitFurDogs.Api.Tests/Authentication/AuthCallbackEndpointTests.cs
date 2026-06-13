using System.Net;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Xunit;

namespace CampFitFurDogs.Api.Tests.Authentication;

public class AuthCallbackEndpointTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    private HttpClient CreateClient(ApiContext ctx)
        => new ApiFactory(ctx).CreateClient(new ApiClientContext());

    // ------------------------------------------------------------
    // SUCCESS
    // ------------------------------------------------------------
    [Fact]
    public async Task Successful_callback_sets_cookie_and_redirects()
    {
        var mappedId = Guid.NewGuid();

        var fakeService = new FakeAuthCallbackService
        {
            CustomerId = CustomerId.From(mappedId),
            RedirectUrl = "http://localhost:3000/"
        };

        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(true)
            .WithServiceOverride(s =>
            {
                s.AddSingleton<IAuthCallbackService>(fakeService);
                s.AddSingleton<IAuthClient, FakeOidcAuthClient>();
            });

        var client = CreateClient(ctx);

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Be("http://localhost:3000/");
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.First().Should().Contain("cfd.session=");
    }

    // ------------------------------------------------------------
    // ERROR PATHS
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_authorization_code_returns_bad_request()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(true);

        var client = CreateClient(ctx);

        var response = await client.GetAsync("/api/auth/callback");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Incomplete_configuration_returns_internal_server_error()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(true)
            .WithConfigOverride(cfg =>
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

        var client = CreateClient(ctx);

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    // ------------------------------------------------------------
    // COOKIE SECURITY
    // ------------------------------------------------------------
    [Fact]
    public void Cookie_is_secure_in_production()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithEnvironment("Production")
            .WithCookieHttpOverride(false);

        var factory = new ApiFactory(ctx);

        using var scope = factory.Services.CreateScope();
        var options = scope.ServiceProvider
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();

        options.Get("cfd.session").Cookie.SecurePolicy
            .Should().Be(CookieSecurePolicy.Always);
    }
}
