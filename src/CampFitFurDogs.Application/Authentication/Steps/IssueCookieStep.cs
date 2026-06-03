using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class IssueCookieStep : IAuthCallbackStep
{
    private readonly ISessionTokenService _tokens;

    public AuthCallbackStepMetadata Metadata =>
        new("IssueCookie", "Issue Cookie");

    public IssueCookieStep(ISessionTokenService tokens)
    {
        _tokens = tokens;
    }

    public bool CanExecute(AuthCallbackContext ctx)
        => true; // Always runs

    public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        var generated = _tokens.Generate();
        var cookie = SessionCookie.FromPlaintextToken(generated.PlaintextToken);

        return Task.FromResult(
            ctx with
            {
                TokenHash = generated.Hash,
                SessionCookie = cookie
            }
        );
    }
}
