using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Frank.Identity.Application.Tests.Fakes.Callback.Oidc;

namespace Frank.Identity.Application.Tests.Callback.Oidc.Steps;

public sealed class ValidateTokensStepTests
{
    [Fact]
    public async Task ExecuteAsync_WhenValidatorThrows_ThrowsOidcProtocolException()
    {
        // Arrange
        var validator = new FakeOidcTokenValidator
        {
            ThrowOnValidate = true
        };

        var step = new ValidateTokensStep(validator);

        var ctx = new CallbackOidcContext
        {
            Code = "abc123",                 // REQUIRED
            IdToken = "fake-id-token",
            Timestamp = DateTimeOffset.UtcNow
        };

        // Act
        var act = async () => await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<OidcProtocolException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidToken_PopulatesSubjectAndClaims()
    {
        // Arrange
        var validator = new FakeOidcTokenValidator
        {
            Result = new OidcTokenValidationResult(
                SubjectId: "sub-123",
                Claims: new Dictionary<string, string>
                {
                    ["email"] = "frank@example.com",
                    ["name"] = "Frank Dog"
                })
        };

        var step = new ValidateTokensStep(validator);

        var ctx = new CallbackOidcContext
        {
            Code = "abc123",                 // REQUIRED
            IdToken = "valid-id-token",
            Timestamp = DateTimeOffset.UtcNow
        };

        // Act
        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        result.SubjectId.Should().Be("sub-123");
        result.Claims.Should().ContainKey("email");
        result.Claims.Should().ContainKey("name");
    }
}
