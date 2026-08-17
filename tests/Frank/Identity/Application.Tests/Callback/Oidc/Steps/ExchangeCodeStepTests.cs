using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Tests.Fakes.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Callback.Oidc.Steps;

public sealed class ExchangeCodeStepTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidResponse_SetsAccessTokenAndIdToken()
    {
        // Arrange
        var fake = new FakeOidcTokenClient
        {
            Result = new OidcTokenResponse(
                AccessToken: "access-123",
                IdToken: "id-456")
        };

        var step = new ExchangeCodeStep(fake);

        var ctx = new CallbackOidcContext
        {
            Code = "abc123",
            Timestamp = DateTimeOffset.UtcNow
        };

        // Act
        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("access-123");
        result.IdToken.Should().Be("id-456");
    }

    [Fact]
    public async Task ExecuteAsync_WhenClientThrows_ThrowsOidcProtocolException()
    {
        // Arrange
        var fake = new FakeOidcTokenClient
        {
            ThrowOnExchange = true
        };

        var step = new ExchangeCodeStep(fake);

        var ctx = new CallbackOidcContext
        {
            Code = "abc123",
            Timestamp = DateTimeOffset.UtcNow
        };

        // Act
        var act = async () => await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OidcProtocolException>();
    }
}
