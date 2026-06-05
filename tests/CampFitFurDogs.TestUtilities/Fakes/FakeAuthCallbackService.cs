using System.Security.Cryptography;
using System.Text;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthCallbackService : IAuthCallbackService
{
    public required CustomerId CustomerId { get; init; }
    public required string RedirectUrl { get; init; }
    public IAuditLogger? AuditLogger { get; init; }

    public Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        // 1. Generate a valid plaintext token (what the cookie stores)
        var plaintextToken = Guid.NewGuid().ToString("N");

        // 2. Hash it using SHA-256 so the domain value object accepts it
        var tokenHash = SessionTokenHash.From(ComputeSha256Hex(plaintextToken));

        // 3. Create a real Session aggregate
        var session = Session.Create(
            tokenHash,
            CustomerId,
            DateTimeOffset.UtcNow
        );

        // 4. Create a real SessionCookie
        var cookie = SessionCookie.FromPlaintextToken(plaintextToken);

        // 5. Fire audit logger if provided
        AuditLogger?.LoginSucceeded(
            CustomerId.Value,
            externalId: "fake-external-id"
        );

        // 6. Return a valid AuthCallbackResult
        return Task.FromResult(new AuthCallbackResult(
            CustomerId,
            cookie,
            RedirectUrl
        ));
    }

    private static string ComputeSha256Hex(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
    }
}
