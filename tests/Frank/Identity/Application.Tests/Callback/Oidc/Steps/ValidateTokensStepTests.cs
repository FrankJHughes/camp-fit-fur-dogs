using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Frank.Identity.Application.Settings;
using Frank.Identity.Application.Tests.Fakes.Callback.Oidc;
using Frank.TestUtilities.Fakes;

namespace Frank.Identity.Application.Tests.Callback.Oidc.Steps;

public sealed class ValidateTokensStepTests
{
    private static OidcCallbackSettings Settings => new()
    {
        Authority = "https://example.auth0.com",
        ClientId = "client-id",
        ClientSecret = "client-secret",
        CallbackUrl = "https://app/callback"
    };

    [Fact]
    public async Task ExecuteAsync_WhenJwksEndpointFails_ThrowsOidcProtocolException()
    {
        // Arrange
        var fake = new FakeOidcHttpClient
        {
            FailJwksEndpoint = true
        };

        var http = fake.CreateClient();
        var step = new ValidateTokensStep(new FakeOptionsMonitor<OidcCallbackSettings>(Settings), http);

        var ctx = new CallbackOidcContext
        {
            Code = "abc123",
            Now = DateTimeOffset.UtcNow,
            IdToken = "fake-id-token"
        };

        // Act
        var act = async () => await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OidcProtocolException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidIdToken_ThrowsOidcProtocolException()
    {
        // Arrange: JWKS succeeds, but token is not a valid JWT
        var fake = new FakeOidcHttpClient
        {
            JwksResponseJson = FakeJwks.Empty
        };

        var http = fake.CreateClient();
        var step = new ValidateTokensStep(new FakeOptionsMonitor<OidcCallbackSettings>(Settings), http);

        var ctx = new CallbackOidcContext
        {
            Code = "abc123",
            Now = DateTimeOffset.UtcNow,
            IdToken = "not-a-jwt"
        };

        // Act
        var act = async () => await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OidcProtocolException>();
    }
}
