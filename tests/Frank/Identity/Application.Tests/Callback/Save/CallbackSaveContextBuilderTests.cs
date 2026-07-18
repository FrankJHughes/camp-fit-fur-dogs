using System.Text.Json;
using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Callback.Save;
using Frank.Core.Application.Tests.Fakes.Application.Authentication.Callback.Steps;
using Frank.TestUtilities.Fakes.Authentication.Callback;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Identity.Application.Tests.Callback.Save;

public sealed class CallbackSaveContextBuilderTests
{
    private static CallbackSaveContextBuilderRequest NewRequest
    {
        get
        {
            // Encode state as JSON
            var returnUrl = "/dashboard";
            var stateObj = new { return_url = returnUrl };
            var stateJson = JsonSerializer.Serialize(stateObj);
            var state = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(stateJson));

            return new()
            {
                External = FakeOidcCallbackResult.Create("sub-123"),
                Now = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
            };
        }
    }

    private static CallbackSaveContextBuilder CreateBuilder(
        params IImmutableContextBuildStep<CallbackSaveContext>[] steps)
        => new(
            steps,
            new FakeObservabilitySink(),
            (_, _) => new FakeObservabilityContext());

    // -------------------------------------------------------------
    // 1. INITIAL CONTEXT CREATION
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_CreatesInitialContextCorrectly()
    {
        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
        {
            new NoOpStep(),
            new SetFinalValuesStep(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "hash",
                "cookie")
        };

        var builder = CreateBuilder(steps);

        var result = await builder.BuildAsync(NewRequest, CancellationToken.None);

        result.Should().NotBeNull();
        result.CookieValue.Should().Be("cookie");
    }

    // -------------------------------------------------------------
    // 2. IMMUTABILITY ENFORCEMENT
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_Throws_WhenStepModifies_External()
    {
        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
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
        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
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
        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
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

        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
        {
            new SetFinalValuesStep(
                customerId,
                sessionId,
                tokenHash: "hash-abc",
                cookieValue: "cookie-xyz")
        };

        var builder = CreateBuilder(steps);

        var result = await builder.BuildAsync(NewRequest, CancellationToken.None);

        result.UserId.Should().Be(customerId);
        result.SessionId.Should().Be(sessionId);
        result.TokenHash.Should().Be("hash-abc");
        result.CookieValue.Should().Be("cookie-xyz");
    }

    // -------------------------------------------------------------
    // 4. STEP EXECUTION ORDER
    // -------------------------------------------------------------
    [Fact]
    public async Task BuildAsync_ExecutesStepsInOrder()
    {
        var recorder = new List<string>();

        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
        {
            new RecordingStep("1", recorder),
            new RecordingStep("2", recorder),
            new RecordingStep("3", recorder),
            new SetFinalValuesStep(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "hash",
                "cookie")
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
        var steps = new IImmutableContextBuildStep<CallbackSaveContext>[]
        {
            new ThrowingStep()
        };

        var builder = CreateBuilder(steps);

        var act = async () => await builder.BuildAsync(NewRequest, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
