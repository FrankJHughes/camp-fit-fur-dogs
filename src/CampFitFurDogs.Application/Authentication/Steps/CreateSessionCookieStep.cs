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

    public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();

        // 1–2. Generate plaintext token + hash via domain-aware service
        var generated = _tokens.Generate();

        // 3. Store hash for CreateSessionStep
        ctx.TokenHash = generated.Hash;

        // 4. Build cookie value using domain VO
        var cookie = SessionCookie.FromPlaintextToken(generated.PlaintextToken);

        // 5. Build result using domain types
        ctx.Result = new AuthCallbackResult(
            CustomerId: CustomerId.From(ctx.CustomerId!.Value),
            Cookie: cookie,
            RedirectUrl: ""
        );

        return Task.CompletedTask;
    }
}
