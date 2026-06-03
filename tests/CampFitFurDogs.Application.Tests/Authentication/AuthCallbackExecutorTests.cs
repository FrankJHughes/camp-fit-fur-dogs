using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Diagnostics;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackExecutorTests
{
    private static AuthToken MakeToken(string value)
        => new AuthToken(value);

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
}
