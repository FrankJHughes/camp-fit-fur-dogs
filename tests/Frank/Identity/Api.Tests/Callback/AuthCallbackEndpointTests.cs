using System.Net;
using System.Text.Json;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Testing.Contexts;
using Frank.TestUtilities.Contexts;
using Frank.TestUtilities.Factories;
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
        : ICallbackOidcContextBuilder
    {
        public CallbackOidcContextBuilderRequest? ReceivedRequest { get; private set; }

        public Task<CallbackOidcContextBuilderResult> BuildAsync(
            CallbackOidcContextBuilderRequest request,
            CancellationToken cancellationToken)
        {
            ReceivedRequest = request;

            return Task.FromResult(new CallbackOidcContextBuilderResult
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
        : ICallbackSaveContextBuilder
    {
        public CallbackSaveContextBuilderRequest? ReceivedRequest { get; private set; }

        public Task<CallbackSaveContextBuilderResult> BuildAsync(
            CallbackSaveContextBuilderRequest request,
            CancellationToken cancellationToken)
        {
            ReceivedRequest = request;

            return Task.FromResult(new CallbackSaveContextBuilderResult
            {
                UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
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
                services.RemoveAll<ICallbackOidcContextBuilder>();

                services.RemoveAll<ICallbackSaveContextBuilder>();

                // Register fakes
                services.AddSingleton<ICallbackOidcContextBuilder, FakeFrankEngine>();

                services.AddSingleton<ICallbackSaveContextBuilder, FakeAppEngine>();
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

        var response = await client.GetAsync($"/api/identity/callback?state={Uri.EscapeDataString(state)}");

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

        var response = await client.GetAsync($"/api/identity/callback?code=abc123&state={Uri.EscapeDataString(state)}");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        response.Headers.Location!.ToString()
            .Should().Be("/dashboard");

        // Cookie issued (domain session cookie)
        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Any(c => c.Contains("session=", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }
}
