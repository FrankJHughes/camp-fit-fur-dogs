using System.Net;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;
using Frank.Authentication.Callback.Oidc;
using Frank.Testing.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CampFitFurDogs.Api.Tests.Authentication.Callback;

public sealed class AuthCallbackEndpointTests : IAsyncLifetime
{
    private ApiFactory _api = default!;
    private ApiContext _ctx = default!;

    // ------------------------------------------------------------
    // FAKES
    // ------------------------------------------------------------

    private sealed class FakeFrankEngine
        : IImmutableContextBuilder<
            FrankAuthCallbackRequest,
            OidcAuthCallbackContext,
            FrankAuthCallbackResult>
    {
        public FrankAuthCallbackRequest? ReceivedRequest { get; private set; }

        public Task<FrankAuthCallbackResult> BuildAsync(
            FrankAuthCallbackRequest request,
            CancellationToken cancellationToken)
        {
            ReceivedRequest = request;

            return Task.FromResult(new FrankAuthCallbackResult
            {
                SubjectId = "sub-123",
                Claims = new Dictionary<string, string>(),
                Email = "john@example.com",
                GivenName = "John",
                FamilyName = "Doe",
                Provider = "test"
            });
        }
    }

    private sealed class FakeAppEngine
        : IImmutableContextBuilder<
            ApplicationAuthCallbackRequest,
            ApplicationAuthCallbackContext,
            ApplicationAuthCallbackContextBuilderResult>
    {
        public ApplicationAuthCallbackRequest? ReceivedRequest { get; private set; }

        public Task<ApplicationAuthCallbackContextBuilderResult> BuildAsync(
            ApplicationAuthCallbackRequest request,
            CancellationToken cancellationToken)
        {
            ReceivedRequest = request;

            return Task.FromResult(new ApplicationAuthCallbackContextBuilderResult
            {
                CustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                SessionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                TokenHash = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                CookieValue = "cookie-value",
                RedirectUrl = "http://localhost:5173/dashboard"
            });
        }
    }

    // ------------------------------------------------------------
    // TEST INITIALIZATION
    // ------------------------------------------------------------
    public Task InitializeAsync()
    {
        _ctx = new ApiContext()
            .WithDatabase(false)
            .WithCookieAuthOnly(false)
            .WithServiceOverride(services =>
            {
                // Remove real engines
                services.RemoveAll<IImmutableContextBuilder<
                    FrankAuthCallbackRequest,
                    OidcAuthCallbackContext,
                    FrankAuthCallbackResult>>();

                services.RemoveAll<IImmutableContextBuilder<
                    ApplicationAuthCallbackRequest,
                    ApplicationAuthCallbackContext,
                    ApplicationAuthCallbackContextBuilderResult>>();

                // Register fakes
                services.AddSingleton<IImmutableContextBuilder<
                    FrankAuthCallbackRequest,
                    OidcAuthCallbackContext,
                    FrankAuthCallbackResult>, FakeFrankEngine>();

                services.AddSingleton<IImmutableContextBuilder<
                    ApplicationAuthCallbackRequest,
                    ApplicationAuthCallbackContext,
                    ApplicationAuthCallbackContextBuilderResult>, FakeAppEngine>();

                // Ensure cookie auth works in TestServer
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    opts.Cookie.SameSite = SameSiteMode.Lax;
                });
            });

        _api = new ApiFactory(_ctx);
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private HttpClient CreateClient() =>
        _api.CreateClient(new ApiClientContext());

    // ------------------------------------------------------------
    // ERROR PATH
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_code_returns_bad_request()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/auth/callback");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Missing authorization code");
    }

    // ------------------------------------------------------------
    // SUCCESS PATH
    // ------------------------------------------------------------
    [Fact]
    public async Task Valid_code_runs_pipelines_issues_cookie_and_redirects()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/auth/callback?code=abc123");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        response.Headers.Location!.ToString()
            .Should().Be("http://localhost:5173/dashboard");

        // Cookie issued
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Any(c => c.Contains("cfd.session", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }
}
