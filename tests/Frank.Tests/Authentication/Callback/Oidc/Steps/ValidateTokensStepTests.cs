using Frank.Authentication.Callback.Oidc;
using Frank.Authentication.Callback.Oidc.Steps;
using Frank.Settings;
using Frank.Tests.Fakes.Authentication.Callback.Oidc;

namespace Frank.Tests.Authentication.Callback.Oidc.Steps;

public sealed class ValidateTokensStepTests
{
    private static AuthCallbackOidcSettings Settings => new()
    {
        Authority = "example.auth0.com",
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
        var step = new ValidateTokensStep(Settings, http);

        var ctx = new OidcAuthCallbackContext
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
        var step = new ValidateTokensStep(Settings, http);

        var ctx = new OidcAuthCallbackContext
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
