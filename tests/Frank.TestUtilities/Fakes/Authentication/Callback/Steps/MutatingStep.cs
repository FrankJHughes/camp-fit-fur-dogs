using System.Threading;
using System.Threading.Tasks;
using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class MutatingStep : IImmutableContextBuildStep<SaveCallbackContext>
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

    public bool CanExecute(SaveCallbackContext ctx) => true;

    public Task<SaveCallbackContext> ExecuteAsync(SaveCallbackContext ctx, CancellationToken ct)
    {
        if (_returnNull)
            return Task.FromResult<SaveCallbackContext>(null!);

        var external = _modifyExternal
            ? FakeOidcCallbackResult.Create("DIFFERENT")
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
