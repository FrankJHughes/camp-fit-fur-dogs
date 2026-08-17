using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Tests.Fakes.Callback.Oidc;
using Frank.Identity.Application.Tests.Fakes.Callback.Oidc.Steps;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Identity.Application.Tests.Callback.Oidc;

public sealed class CallbackOidcContextBuilderTests
{
    private static CallbackOidcContextBuilder CreateBuilder(
        params IImmutableContextBuildStep<CallbackOidcContext>[] steps)
        => new(
            steps,
            new FakeObservabilitySink(),
            (_, _) => new FakeObservabilityContext());

    [Fact]
    public async Task ProcessAsync_WithValidFlow_ProducesOidcCallbackResult()
    {
        var steps = new IImmutableContextBuildStep<CallbackOidcContext>[]
        {
            new FakeExchangeCodeStep("access-token"),
            new FakeFetchUserInfoStep(FakeUserInfo.Basic),
            new FakeValidateTokensStep()
        };

        var engine = CreateBuilder(steps);

        var request = new CallbackOidcContextBuilderRequest
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
        var steps = new IImmutableContextBuildStep<CallbackOidcContext>[]
        {
            new ThrowingStep()
        };

        var engine = CreateBuilder(steps);

        var request = new CallbackOidcContextBuilderRequest
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

        var steps = new IImmutableContextBuildStep<CallbackOidcContext>[]
        {
            new RecordingStep("1", recorder),
            new RecordingStep("2", recorder),
            new RecordingStep("3", recorder)
        };

        var engine = CreateBuilder(steps);

        var request = new CallbackOidcContextBuilderRequest
        {
            Code = "abc123"
        };

        var act = async () => await engine.BuildAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        recorder.Should().Equal(new[] { "1" });
    }
}
