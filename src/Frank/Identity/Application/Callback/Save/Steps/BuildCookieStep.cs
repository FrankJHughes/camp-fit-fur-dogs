using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Authentication;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Callback.Save.Steps;

public sealed class BuildCookieStep
    : IImmutableContextBuildStep<SaveCallbackContext>
{
    private readonly ISessionTokenService _tokens;

    public BuildCookieStep(ISessionTokenService tokens)
    {
        _tokens = tokens;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "BuildCookie",
            displayName: "Build Cookie"
        );

    public bool CanExecute(SaveCallbackContext ctx)
        => ctx.CookieValue is null; // runs once, before session creation

    public Task<SaveCallbackContext> ExecuteAsync(
        SaveCallbackContext ctx,
        CancellationToken ct)
    {
        // 1. Generate token + hash
        var generated = _tokens.Generate();

        // 2. Build the cookie value
        var cookie = SessionCookie.FromPlaintextToken(generated.PlaintextToken);

        // 3. Return updated context
        return Task.FromResult(
            ctx with
            {
                TokenHash = generated.HashedToken.Value,
                CookieValue = cookie.Value
            }
        );
    }
}
