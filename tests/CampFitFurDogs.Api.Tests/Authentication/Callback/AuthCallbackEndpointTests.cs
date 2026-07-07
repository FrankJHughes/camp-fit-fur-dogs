using System.Net;
using System.Text.Json;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;
using Frank.Authentication.Callback.Oidc;
using Frank.Testing.Contexts;
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
    public async Task Missing_code_returns_without_session()
    {
        var client = CreateClient();

        // Encode state as JSON
        var returnUrl = "/dashboard";
        var stateObj = new { return_url = returnUrl };
        var stateJson = JsonSerializer.Serialize(stateObj);
        var state = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(stateJson));

        var response = await client.GetAsync($"/api/auth/callback?state={Uri.EscapeDataString(state)}");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeFalse();
    }

    // ------------------------------------------------------------
    // SUCCESS PATH
    // ------------------------------------------------------------
    [Fact]
    public async Task Valid_code_runs_pipelines_issues_cookie_and_redirects()
    {
        var client = CreateClient();

        // Encode state as JSON
        var returnUrl = "/dashboard";
        var stateObj = new { return_url = returnUrl };
        var stateJson = JsonSerializer.Serialize(stateObj);
        var state = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(stateJson));

        var response = await client.GetAsync($"/api/auth/callback?code=abc123&state={Uri.EscapeDataString(state)}");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        response.Headers.Location!.ToString()
            .Should().Be("/dashboard");

        // Cookie issued (domain session cookie)
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Any(c => c.Contains("session=", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }
}
