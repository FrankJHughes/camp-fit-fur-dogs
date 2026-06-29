using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;
using Frank.Tests.Fakes.Application.Authentication.Callback.Steps;
using CampFitFurDogs.TestUtilities.Fakes.Observability;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback;

public sealed class ApplicationAuthCallbackContextBuilderTests
{
    private static ApplicationAuthCallbackRequest NewRequest => new()
    {
        External = FakeFrankAuthCallbackResult.Create("sub-123"),
        Now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
        RequestedRedirectUrl = "/dashboard"
    };

    private static ApplicationAuthCallbackContextBuilder CreateBuilder(
        params IImmutableContextBuildStep<ApplicationAuthCallbackContext>[] steps)
        => new ApplicationAuthCallbackContextBuilder(
            steps,
            new FakeObservabilitySink(),
            new FakeObservabilityContext());

    // -------------------------------------------------------------
    // 1. INITIAL CONTEXT CREATION
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_CreatesInitialContextCorrectly()
    {
        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new NoOpStep(),
            new SetFinalValuesStep(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "hash",
                "cookie",
                "/dashboard")
        };

        var builder = CreateBuilder(steps);

        var result = await builder.BuildAsync(NewRequest, CancellationToken.None);

        result.Should().NotBeNull();
        result.RedirectUrl.Should().Be("/dashboard");
    }

    // -------------------------------------------------------------
    // 2. IMMUTABILITY ENFORCEMENT
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_Throws_WhenStepModifies_External()
    {
        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new MutatingStep(modifyExternal: true)
        };

        var builder = CreateBuilder(steps);

        var act = async () => await builder.BuildAsync(NewRequest, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*immutable field 'External'*");
    }

    [Fact]
    public async Task BuildAsync_Throws_WhenStepModifies_Now()
    {
        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new MutatingStep(modifyNow: true)
        };

        var builder = CreateBuilder(steps);

        var act = async () => await builder.BuildAsync(NewRequest, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*immutable field 'Now'*");
    }

    [Fact]
    public async Task BuildAsync_Throws_WhenStepReturnsNull()
    {
        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new MutatingStep(returnNull: true)
        };

        var builder = CreateBuilder(steps);

        var act = async () => await builder.BuildAsync(NewRequest, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*returned null context*");
    }

    // -------------------------------------------------------------
    // 3. RESULT MAPPING
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_MapsFinalContextToResult()
    {
        var customerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var sessionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new SetFinalValuesStep(
                customerId,
                sessionId,
                tokenHash: "hash-abc",
                cookieValue: "cookie-xyz",
                redirectUrl: "/final")
        };

        var builder = CreateBuilder(steps);

        var result = await builder.BuildAsync(NewRequest, CancellationToken.None);

        result.CustomerId.Should().Be(customerId);
        result.SessionId.Should().Be(sessionId);
        result.TokenHash.Should().Be("hash-abc");
        result.CookieValue.Should().Be("cookie-xyz");
        result.RedirectUrl.Should().Be("/final");
    }

    // -------------------------------------------------------------
    // 4. STEP EXECUTION ORDER
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_ExecutesStepsInOrder()
    {
        var recorder = new List<string>();

        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new RecordingStep("1", recorder),
            new RecordingStep("2", recorder),
            new RecordingStep("3", recorder),
            new SetFinalValuesStep(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "hash",
                "cookie",
                "/redirect")
        };

        var builder = CreateBuilder(steps);

        await builder.BuildAsync(NewRequest, CancellationToken.None);

        recorder.Should().Equal(new[] { "1", "2", "3" });
    }

    // -------------------------------------------------------------
    // 5. EXCEPTION PROPAGATION
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_WhenStepThrows_PropagatesException()
    {
        var steps = new IImmutableContextBuildStep<ApplicationAuthCallbackContext>[]
        {
            new ThrowingStep()
        };

        var builder = CreateBuilder(steps);

        var act = async () => await builder.BuildAsync(NewRequest, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
