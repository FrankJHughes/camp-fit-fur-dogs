using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Authentication.Callback.Oidc;
using Frank.Tests.Fakes.Authentication.Callback.Oidc;
using Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;

namespace Frank.Tests.Authentication.Callback.Oidc;

public sealed class OidcAuthCallbackContextBuilderTests
{
    [Fact]
    public async Task ProcessAsync_WithValidFlow_ProducesFrankAuthCallbackResult()
    {
        // Arrange: a full fake pipeline
        var steps = new IImmutableContextBuildStep<OidcAuthCallbackContext>[]
        {
            new FakeExchangeCodeStep("access-token"),
            new FakeFetchUserInfoStep(FakeUserInfo.Basic),
            new FakeValidateTokensStep()
        };

        var engine = new OidcAuthCallbackContextBuilder(steps);

        var request = new FrankAuthCallbackRequest
        {
            Code = "abc123"
        };

        // Act
        var result = await engine.BuildAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SubjectId.Should().Be("user-123");
        result.GivenName.Should().Be("Test");
        result.FamilyName.Should().Be("User");
        result.Email.Should().Be("test@campfitfurdogs.com");
        result.Claims.Should().ContainKey("sub");
    }

    [Fact]
    public async Task ProcessAsync_WhenStepThrows_EnginePropagatesException()
    {
        var steps = new IImmutableContextBuildStep<OidcAuthCallbackContext>[]
        {
            new ThrowingStep()
        };

        var engine = new OidcAuthCallbackContextBuilder(steps);

        var request = new FrankAuthCallbackRequest
        {
            Code = "abc123"
        };

        var act = async () => await engine.BuildAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ProcessAsync_ExecutesStepsInOrder()
    {
        var recorder = new List<string>();

        var steps = new IImmutableContextBuildStep<OidcAuthCallbackContext>[]
        {
        new RecordingStep("1", recorder),
        new RecordingStep("2", recorder),
        new RecordingStep("3", recorder)
        };

        var engine = new OidcAuthCallbackContextBuilder(steps);

        var request = new FrankAuthCallbackRequest
        {
            Code = "abc123"
        };

        var act = async () => await engine.BuildAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        // Only the first step runs before the exception
        recorder.Should().Equal(new[] { "1" });
    }
}
