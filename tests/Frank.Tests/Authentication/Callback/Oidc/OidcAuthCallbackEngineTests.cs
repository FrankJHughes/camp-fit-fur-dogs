using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Authentication.Callback.Oidc;
using Frank.Tests.Fakes.Authentication.Callback.Oidc;
using Frank.Tests.Fakes.Authentication.Callback.Oidc.Steps;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Tests.Authentication.Callback.Oidc;

public sealed class OidcAuthCallbackContextBuilderTests
{
    private static OidcAuthCallbackContextBuilder CreateBuilder(
        params IImmutableContextBuildStep<OidcAuthCallbackContext>[] steps)
        => new OidcAuthCallbackContextBuilder(
            steps,
            new FakeObservabilitySink(),
            new FakeObservabilityContext());

    [Fact]
    public async Task ProcessAsync_WithValidFlow_ProducesFrankAuthCallbackResult()
    {
        var steps = new IImmutableContextBuildStep<OidcAuthCallbackContext>[]
        {
            new FakeExchangeCodeStep("access-token"),
            new FakeFetchUserInfoStep(FakeUserInfo.Basic),
            new FakeValidateTokensStep()
        };

        var engine = CreateBuilder(steps);

        var request = new FrankAuthCallbackRequest
        {
            Code = "abc123"
        };

        var result = await engine.BuildAsync(request, CancellationToken.None);

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

        var engine = CreateBuilder(steps);

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

        var engine = CreateBuilder(steps);

        var request = new FrankAuthCallbackRequest
        {
            Code = "abc123"
        };

        var act = async () => await engine.BuildAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        recorder.Should().Equal(new[] { "1" });
    }
}
