using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Frank.Identity.Application.Settings;
using Frank.Identity.Application.Tests.Fakes.Callback.Oidc;
using Frank.TestUtilities.Fakes;

namespace Frank.Identity.Application.Tests.Callback.Oidc.Steps;

public sealed class ExchangeCodeStepTests
{
    private static OidcCallbackSettings Settings => new()
    {
        Authority = "https://example.auth0.com",
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

        // FIX: wrap settings in OptionsMonitorFake
        var step = new ExchangeCodeStep(
            http,
            new FakeOptionsMonitor<OidcCallbackSettings>(Settings)
        );

        var ctx = new OidcCallbackContext
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

        // FIX: wrap settings in OptionsMonitorFake
        var step = new ExchangeCodeStep(
            http,
            new FakeOptionsMonitor<OidcCallbackSettings>(Settings)
        );

        var ctx = new OidcCallbackContext
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
