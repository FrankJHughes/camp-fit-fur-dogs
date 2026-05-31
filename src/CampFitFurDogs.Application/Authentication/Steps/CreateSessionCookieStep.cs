using System.Security.Cryptography;
using System.Text;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class CreateSessionCookieStep : IAuthCallbackStep
{
    public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();

        // 1. Generate secure random plaintext token
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToHexString(tokenBytes).ToLowerInvariant();

        // 2. Hash for DB storage
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        var tokenHash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        // 3. Store hash for CreateSessionStep
        ctx.TokenHash = SessionTokenHash.From(tokenHash);

        // 4. Build cookie value using domain VO
        var cookie = SessionCookie.FromPlaintextToken(token);

        // 5. Build result using domain types
        ctx.Result = new AuthCallbackResult(
            CustomerId: CustomerId.From(ctx.CustomerId!.Value),
            Cookie: cookie,
            RedirectUrl: ""
        );

        return Task.CompletedTask;
    }
}
