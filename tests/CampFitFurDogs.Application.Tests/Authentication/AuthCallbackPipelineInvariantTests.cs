using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackPipelineInvariantTests
{
    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private sealed class NullReturningStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("NullStep", "Null Step");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult<AuthCallbackContext>(null!);
        }
    }

    private sealed class CodeMutatingStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("CodeMutate", "Code Mutate");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx with { Code = "different" });
        }
    }

    private sealed class NowMutatingStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("NowMutate", "Now Mutate");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx with { Now = ctx.Now.AddMinutes(1) });
        }
    }

    private sealed class ClearingFieldStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("ClearField", "Clear Field");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx with { Token = null });
        }
    }

    private sealed class SameInstanceStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("SameInstance", "Same Instance");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx); // allowed
        }
    }

    private sealed class RefinementStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("Refine", "Refine");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx with { RedirectUrl = "/next" });
        }
    }

    // ------------------------------------------------------------
    // 1. Reject null context
    // ------------------------------------------------------------
    [Fact]
    public async Task Throws_when_step_returns_null()
    {
        var steps = new IAuthCallbackStep[]
        {
            new NullReturningStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        Func<Task> act = () => pipeline.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*returned null*");
    }

    // ------------------------------------------------------------
    // 2. Reject Code mutation
    // ------------------------------------------------------------
    [Fact]
    public async Task Throws_when_step_mutates_Code()
    {
        var steps = new IAuthCallbackStep[]
        {
            new CodeMutatingStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        Func<Task> act = () => pipeline.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Code*");
    }

    // ------------------------------------------------------------
    // 3. Reject Now mutation
    // ------------------------------------------------------------
    [Fact]
    public async Task Throws_when_step_mutates_Now()
    {
        var steps = new IAuthCallbackStep[]
        {
            new NowMutatingStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code", Now: DateTimeOffset.UtcNow);

        Func<Task> act = () => pipeline.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Now*");
    }

    // ------------------------------------------------------------
    // 4. Reject clearing previously-set fields
    // ------------------------------------------------------------
    [Fact]
    public async Task Throws_when_step_clears_previously_set_field()
    {
        var initial = new AuthCallbackContext("code", Token: new AuthToken("abc"));

        var steps = new IAuthCallbackStep[]
        {
            new ClearingFieldStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);

        Func<Task> act = () => pipeline.ExecuteAsync(initial, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cleared*Token*");
    }

    // ------------------------------------------------------------
    // 5. Allow returning same instance
    // ------------------------------------------------------------
    [Fact]
    public async Task Allows_returning_same_instance()
    {
        var steps = new IAuthCallbackStep[]
        {
            new SameInstanceStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        var result = await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        result.Should().BeSameAs(ctx);
    }

    // ------------------------------------------------------------
    // 6. Allow refinement (setting previously-null fields)
    // ------------------------------------------------------------
    [Fact]
    public async Task Allows_setting_previously_null_fields()
    {
        var steps = new IAuthCallbackStep[]
        {
            new RefinementStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        var result = await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        result.RedirectUrl.Should().Be("/next");
    }
}
