using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class MutatingStep : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly bool _modifyExternal;
    private readonly bool _modifyNow;
    private readonly bool _returnNull;

    public MutatingStep(bool modifyExternal = false, bool modifyNow = false, bool returnNull = false)
    {
        _modifyExternal = modifyExternal;
        _modifyNow = modifyNow;
        _returnNull = returnNull;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("Mutate", "Mutating Step");

    public bool CanExecute(ApplicationAuthCallbackContext ctx) => true;

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(ApplicationAuthCallbackContext ctx, CancellationToken ct)
    {
        if (_returnNull)
            return Task.FromResult<ApplicationAuthCallbackContext>(null!);

        var external = _modifyExternal
            ? FakeFrankAuthCallbackResult.Create("DIFFERENT")
            : ctx.External;

        var now = _modifyNow ? ctx.Now.AddMinutes(5) : ctx.Now;

        return Task.FromResult(
            ctx with
            {
                External = external,
                Now = now
            }
        );
    }
}
