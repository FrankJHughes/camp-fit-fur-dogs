using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Diagnostics;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackExecutorTests
{
    private static AuthToken MakeToken(string value)
        => new AuthToken(value);

    private static AuthUser MakeUser()
        => new AuthUser("id", "First", "Last", "email@example.com");

    private static AuthCallbackContext Ctx(Action<AuthCallbackContextBuilder> apply)
    {
        var b = new AuthCallbackContextBuilder();
        apply(b);
        return b.Build();
    }

    // ------------------------------------------------------------
    // 1. Executes steps in correct order
    // ------------------------------------------------------------
    [Fact]
    public async Task ExecutesStepsInOrder()
    {
        var calls = new List<string>();

        var step1 = new FakeAuthCallbackStep(
            "A",
            canExecute: _ => true,
            exec: ctx =>
            {
                calls.Add("A");
                return ctx;
            });

        var step2 = new FakeAuthCallbackStep(
            "B",
            canExecute: _ => true,
            exec: ctx =>
            {
                calls.Add("B");
                return ctx;
            });

        var executor = new AuthCallbackExecutor(new[] { step1, step2 });

        await executor.ExecuteAsync(Ctx(_ => { }), default);

        calls.Should().Equal("A", "B");
    }

    // ------------------------------------------------------------
    // 2. Steps only run when CanExecute is true
    // ------------------------------------------------------------
    [Fact]
    public async Task SkipsStepsUntilCanExecuteIsTrue()
    {
        var calls = new List<string>();

        var step1 = new FakeAuthCallbackStep(
            "A",
            canExecute: _ => false,
            exec: ctx =>
            {
                calls.Add("A");
                return ctx;
            });

        var step2 = new FakeAuthCallbackStep(
            "B",
            canExecute: _ => true,
            exec: ctx =>
            {
                calls.Add("B");
                return ctx;
            });

        var executor = new AuthCallbackExecutor(new[] { step1, step2 });

        await executor.ExecuteAsync(Ctx(_ => { }), default);

        calls.Should().Equal("B");
    }

    // ------------------------------------------------------------
    // 3. Immutable fields cannot be modified
    // ------------------------------------------------------------
    [Fact]
    public async Task ThrowsIfImmutableFieldIsModified()
    {
        var ctx = Ctx(b => b.Code("abc"));

        var badStep = new FakeAuthCallbackStep(
            "Bad",
            canExecute: _ => true,
            exec: _ => Ctx(b => b.Code("changed")));

        var executor = new AuthCallbackExecutor(new[] { badStep });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(ctx, default));
    }

    // ------------------------------------------------------------
    // 4. Steps cannot clear previously set fields
    // ------------------------------------------------------------
    [Fact]
    public async Task ThrowsIfStepClearsAField()
    {
        var ctx = Ctx(b => b.Token(MakeToken("xyz")));

        var badStep = new FakeAuthCallbackStep(
            "Bad",
            canExecute: _ => true,
            exec: _ => Ctx(_ => { })); // clears Token

        var executor = new AuthCallbackExecutor(new[] { badStep });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(ctx, default));
    }

    // ------------------------------------------------------------
    // 5. Diagnostics fire Start and End events
    // ------------------------------------------------------------
    [Fact]
    public async Task EmitsDiagnosticsForEachStep()
    {
        var events = new List<AuthCallbackDiagnosticEvent>();

        var step = new FakeAuthCallbackStep(
            "A",
            canExecute: _ => true,
            exec: ctx => ctx);

        var executor = new AuthCallbackExecutor(
            new[] { step },
            trace: evt => events.Add(evt));

        await executor.ExecuteAsync(Ctx(_ => { }), default);

        events.Should().HaveCount(2);

        events[0].Phase.Should().Be("Start");
        events[1].Phase.Should().Be("End");
        events[1].DurationMs.Should().NotBeNull();
    }

    // ------------------------------------------------------------
    // 6. Null return from step throws
    // ------------------------------------------------------------
    [Fact]
    public async Task ThrowsIfStepReturnsNull()
    {
        var badStep = new FakeAuthCallbackStep(
            "Bad",
            canExecute: _ => true,
            exec: _ => null!);

        var executor = new AuthCallbackExecutor(new[] { badStep });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(Ctx(_ => { }), default));
    }

    // ------------------------------------------------------------
    // 7. Step throws exception → executor must propagate
    // ------------------------------------------------------------
    [Fact]
    public async Task PropagatesExceptionsThrownByStep()
    {
        var step = new FakeAuthCallbackStep(
            "Boom",
            canExecute: _ => true,
            exec: _ => throw new InvalidOperationException("boom"));

        var executor = new AuthCallbackExecutor(new[] { step });

        var ctx = Ctx(_ => { });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(ctx, default));

        ex.Message.Should().Be("boom");
    }

    // ------------------------------------------------------------
    // 8. Step returns same context instance → allowed
    // ------------------------------------------------------------
    [Fact]
    public async Task AllowsReturningSameContextInstance()
    {
        var ctx = Ctx(_ => { });

        var step = new FakeAuthCallbackStep(
            "Same",
            canExecute: _ => true,
            exec: before => before); // same instance

        var executor = new AuthCallbackExecutor(new[] { step });

        await executor.ExecuteAsync(ctx, default);
    }

    // ------------------------------------------------------------
    // 9. Step sets multiple fields → allowed
    // ------------------------------------------------------------
    [Fact]
    public async Task AllowsSettingMultipleFields()
    {
        var ctx = Ctx(_ => { });

        var step = new FakeAuthCallbackStep(
            "Multi",
            canExecute: _ => true,
            exec: before => Ctx(b =>
            {
                b.Code(before.Code);
                b.Now(before.Now);
                b.Token(MakeToken("t"));
                b.User(MakeUser());
            }));

        var executor = new AuthCallbackExecutor(new[] { step });

        await executor.ExecuteAsync(ctx, default);
    }

    // ------------------------------------------------------------
    // 10. Diagnostics: Start/End pairs match step count
    // ------------------------------------------------------------
    [Fact]
    public async Task EmitsMatchingStartEndPairs()
    {
        var events = new List<AuthCallbackDiagnosticEvent>();

        var step1 = new FakeAuthCallbackStep("A", _ => true, ctx => ctx);
        var step2 = new FakeAuthCallbackStep("B", _ => true, ctx => ctx);

        var executor = new AuthCallbackExecutor(
            new[] { step1, step2 },
            trace: evt => events.Add(evt));

        await executor.ExecuteAsync(Ctx(_ => { }), default);

        events.Should().HaveCount(4);

        events.Where(e => e.Phase == "Start").Should().HaveCount(2);
        events.Where(e => e.Phase == "End").Should().HaveCount(2);

        // Ensure ordering: Start A, End A, Start B, End B
        events[0].StepId.Should().Be("A");
        events[0].Phase.Should().Be("Start");

        events[1].StepId.Should().Be("A");
        events[1].Phase.Should().Be("End");

        events[2].StepId.Should().Be("B");
        events[2].Phase.Should().Be("Start");

        events[3].StepId.Should().Be("B");
        events[3].Phase.Should().Be("End");
    }

    // ------------------------------------------------------------
    // 11. No steps can execute → executor completes silently
    // ------------------------------------------------------------
    [Fact]
    public async Task CompletesWhenNoStepsCanExecute()
    {
        var step = new FakeAuthCallbackStep(
            "Never",
            canExecute: _ => false,
            exec: ctx => ctx);

        var executor = new AuthCallbackExecutor(new[] { step });

        await executor.ExecuteAsync(Ctx(_ => { }), default);
    }

    // ------------------------------------------------------------
    // 12. Step adds field after it was previously null → allowed
    // ------------------------------------------------------------
    [Fact]
    public async Task AllowsSettingPreviouslyNullField()
    {
        var ctx = Ctx(_ => { });

        var step = new FakeAuthCallbackStep(
            "AddToken",
            canExecute: _ => true,
            exec: before => Ctx(b =>
            {
                b.Code(before.Code);
                b.Now(before.Now);
                b.Token(MakeToken("abc"));
            }));

        var executor = new AuthCallbackExecutor(new[] { step });

        await executor.ExecuteAsync(ctx, default);
    }

    // ------------------------------------------------------------
    // 13. Step modifies multiple immutable fields → throws
    // ------------------------------------------------------------
    [Fact]
    public async Task ThrowsIfMultipleImmutableFieldsAreModified()
    {
        var ctx = Ctx(b => b.Code("abc"));

        var step = new FakeAuthCallbackStep(
            "Bad",
            canExecute: _ => true,
            exec: _ => Ctx(b =>
            {
                b.Code("changed");
                b.Now(DateTimeOffset.UtcNow.AddMinutes(1));
            }));

        var executor = new AuthCallbackExecutor(new[] { step });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(ctx, default));
    }

    // ------------------------------------------------------------
    // 14. Step clears multiple fields → throws
    // ------------------------------------------------------------
    [Fact]
    public async Task ThrowsIfStepClearsMultipleFields()
    {
        var ctx = Ctx(b =>
        {
            b.Token(MakeToken("t"));
            b.User(MakeUser());
        });

        var step = new FakeAuthCallbackStep(
            "Bad",
            canExecute: _ => true,
            exec: _ => Ctx(_ => { })); // clears both

        var executor = new AuthCallbackExecutor(new[] { step });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync(ctx, default));
    }

    // ------------------------------------------------------------
    // 15. Diagnostics: Before/After snapshots are correct
    // ------------------------------------------------------------
    [Fact]
    public async Task DiagnosticsContainCorrectBeforeAfterSnapshots()
    {
        var events = new List<AuthCallbackDiagnosticEvent>();

        var step = new FakeAuthCallbackStep(
            "A",
            _ => true,
            ctx => Ctx(b =>
            {
                b.Code(ctx.Code);
                b.Now(ctx.Now);
                b.Token(new AuthToken("new"));
            }));

        var executor = new AuthCallbackExecutor(
            new[] { step },
            trace: evt => events.Add(evt));

        var initial = Ctx(b => b.Code("abc"));

        await executor.ExecuteAsync(initial, default);

        var start = events[0];
        var end = events[1];

        start.Before.Code.Should().Be("abc");
        start.After.Code.Should().Be("abc");

        end.Before.Code.Should().Be("abc");
        end.After.Token!.AccessToken.Should().Be("new");
    }

    // ------------------------------------------------------------
    // Step becomes executable only after another step mutates context
    // ------------------------------------------------------------
    [Fact]
    public async Task StepBecomesExecutableAfterAnotherStepRuns()
    {
        var calls = new List<string>();

        // Step A: always runs, sets Token
        var stepA = new FakeAuthCallbackStep(
            "A",
            canExecute: _ => true,
            exec: before => Ctx(b =>
            {
                b.Code(before.Code);
                b.Now(before.Now);
                b.Token(MakeToken("abc"));
                calls.Add("A");
            }));

        // Step B: only runs if Token is set
        var stepB = new FakeAuthCallbackStep(
            "B",
            canExecute: ctx => ctx.Token is not null,
            exec: before => Ctx(b =>
            {
                b.Code(before.Code);
                b.Now(before.Now);
                b.Token(before.Token!); // ← REQUIRED
                calls.Add("B");
            }));

        var executor = new AuthCallbackExecutor(new[] { stepA, stepB });

        await executor.ExecuteAsync(Ctx(_ => { }), default);

        calls.Should().Equal("A", "B");
    }

    // ------------------------------------------------------------
    // Pipeline halts early when no remaining steps can execute
    // ------------------------------------------------------------
    [Fact]
    public async Task HaltsWhenNoFurtherStepsCanExecute()
    {
        var calls = new List<string>();

        // Step A: runs once
        var stepA = new FakeAuthCallbackStep(
            "A",
            canExecute: _ => true,
            exec: before => Ctx(b =>
            {
                b.Code(before.Code);
                b.Now(before.Now);
                calls.Add("A");
            }));

        // Step B: can never run (requires Token, but A doesn't set it)
        var stepB = new FakeAuthCallbackStep(
            "B",
            canExecute: ctx => ctx.Token is not null,
            exec: before => Ctx(b =>
            {
                b.Code(before.Code);
                b.Now(before.Now);
                calls.Add("B");
            }));

        var executor = new AuthCallbackExecutor(new[] { stepA, stepB });

        await executor.ExecuteAsync(Ctx(_ => { }), default);

        // Only A runs; executor halts because B can never run
        calls.Should().Equal("A");
    }

}
