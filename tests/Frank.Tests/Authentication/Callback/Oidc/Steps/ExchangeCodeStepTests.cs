using Frank.Authentication.Callback.Oidc;
using Frank.Authentication.Callback.Oidc.Steps;
using Frank.Tests.Fakes.Authentication.Callback.Oidc;
using Xunit;

namespace Frank.Tests.Authentication.Callback.Oidc.Steps;

public sealed class ExchangeCodeStepTests
{
    private static OidcAuthCallbackOptions Options => new()
    {
        Authority = "example.auth0.com",
        ClientId = "client-id",
        ClientSecret = "client-secret",
        CallbackUrl = "https://app/callback"
    };

    [Fact]
    public async Task ExecuteAsync_WithValidResponse_SetsAccessTokenAndIdToken()
    {
        // Arrange
        var fake = new FakeOidcHttpClient
        {
            TokenResponseJson = """
            {
                "access_token": "access-123",
                "id_token": "id-456"
            }
            """
        };

        var http = fake.CreateClient();
        var step = new ExchangeCodeStep(http, Options);

        var ctx = new OidcAuthCallbackContext
        {
            Code = "abc123",
            Now = DateTimeOffset.UtcNow
        };

        // Act
        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("access-123");
        result.IdToken.Should().Be("id-456");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTokenEndpointFails_ThrowsOidcProtocolException()
    {
        // Arrange
        var fake = new FakeOidcHttpClient
        {
            FailTokenEndpoint = true
        };

        var http = fake.CreateClient();
        var step = new ExchangeCodeStep(http, Options);

        var ctx = new OidcAuthCallbackContext
        {
            Code = "abc123",
            Now = DateTimeOffset.UtcNow
        };

        // Act
        var act = async () => await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OidcProtocolException>();
    }
}
