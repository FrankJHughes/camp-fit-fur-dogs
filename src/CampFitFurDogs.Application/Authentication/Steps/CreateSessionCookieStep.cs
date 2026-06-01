using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class CreateSessionCookieStep : IAuthCallbackStep
{
    private readonly ISessionTokenService _tokens;

    public StepMetadata Metadata =>
        new("CreateSessionCookie", "Create Session Cookie");

    public CreateSessionCookieStep(ISessionTokenService tokens)
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
