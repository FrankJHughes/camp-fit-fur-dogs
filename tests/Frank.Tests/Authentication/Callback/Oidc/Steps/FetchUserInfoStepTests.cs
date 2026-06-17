using Frank.Authentication.Callback.Oidc;
using Frank.Authentication.Callback.Oidc.Steps;
using Frank.Tests.Fakes.Authentication.Callback.Oidc;

namespace Frank.Tests.Authentication.Callback.Oidc.Steps;

public sealed class FetchUserInfoStepTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidAccessToken_PopulatesContext()
    {
        // Arrange
        var fakeClient = new FakeOidcUserInfoClient
        {
            Response = FakeUserInfo.Basic
        };

        var step = new FetchUserInfoStep(fakeClient);

        var ctx = new OidcAuthCallbackContext
        {
            Code = "abc123",
            Now = DateTimeOffset.UtcNow,
            AccessToken = "access-token-xyz"
        };

        // Act
        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        // Assert
        result.SubjectId.Should().Be("user-123");
        result.Email.Should().Be("test@campfitfurdogs.com");
        result.GivenName.Should().Be("Test");
        result.FamilyName.Should().Be("User");
        result.Claims.Should().ContainKey("sub");
    }

    [Fact]
    public async Task ExecuteAsync_WhenClientThrows_PropagatesException()
    {
        var fakeClient = new FakeOidcUserInfoClient
        {
            ShouldThrow = true
        };

        var step = new FetchUserInfoStep(fakeClient);

        var ctx = new OidcAuthCallbackContext
        {
            Code = "abc123",
            Now = DateTimeOffset.UtcNow,
            AccessToken = "access-token-xyz"
        };

        var act = async () => await step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
