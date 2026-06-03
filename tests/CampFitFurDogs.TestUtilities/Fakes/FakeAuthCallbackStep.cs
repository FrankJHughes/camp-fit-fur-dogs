using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthCallbackStep : IAuthCallbackStep
{
    private readonly Func<AuthCallbackContext, bool> _canExecute;
    private readonly Func<AuthCallbackContext, AuthCallbackContext> _exec;

    public FakeAuthCallbackStep(
        string id,
        Func<AuthCallbackContext, bool> canExecute,
        Func<AuthCallbackContext, AuthCallbackContext> exec)
    {
        Metadata = new AuthCallbackStepMetadata(
            id,
            id);

        _canExecute = canExecute;
        _exec = exec;
    }

    public AuthCallbackStepMetadata Metadata { get; }

    public bool CanExecute(AuthCallbackContext ctx) => _canExecute(ctx);

    public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        => Task.FromResult(_exec(ctx));
}

