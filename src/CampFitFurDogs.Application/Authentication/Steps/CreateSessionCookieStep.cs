using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class CreateSessionCookieStep : IAuthCallbackStep
{
    private readonly ISessionTokenService _tokens;

    public CreateSessionCookieStep(ISessionTokenService tokens)
    {
        _tokens = tokens;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();
        var generated = _tokens.Generate();
        ctx.TokenHash = generated.Hash;
        var cookie = SessionCookie.FromPlaintextToken(generated.PlaintextToken);
        ctx.SessionCookie = cookie;
        return ctx;
    }
}
