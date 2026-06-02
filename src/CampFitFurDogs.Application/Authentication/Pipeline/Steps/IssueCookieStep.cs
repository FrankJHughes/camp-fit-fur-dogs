using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps
;

public sealed class IssueCookieStep : IAuthCallbackStep
{
    private readonly ISessionTokenService _tokens;

    public AuthCallbackStepMetadata Metadata =>
        new(
            "IssueCookie",
            "Issue Cookie",
            AuthCallbackStepCategory.IssueCookie);

    public IssueCookieStep(ISessionTokenService tokens)
    {
        _tokens = tokens;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        var generated = _tokens.Generate();
        var cookie = SessionCookie.FromPlaintextToken(generated.PlaintextToken);

        return ctx with { TokenHash = generated.Hash, SessionCookie = cookie };
    }
}
