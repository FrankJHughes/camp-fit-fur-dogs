using Frank.Abstractions.ImmutableContext;
using Frank.Application.Abstractions.Authentication;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Domain.Sessions;

namespace Frank.Application.Identity.Callback.Steps;

public sealed class BuildCookieStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
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

    public bool CanExecute(ApplicationAuthCallbackContext ctx)
        => ctx.CookieValue is null; // runs once, before session creation

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
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
