using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class CreateSessionCookieStepTests
{
    // ------------------------------------------------------------
    // Fake token service
    // ------------------------------------------------------------
    private sealed class FakeTokenService : ISessionTokenService
    {
        public GeneratedSessionToken Generate()
        {
            // Deterministic plaintext token (64 hex chars)
            var plaintext =
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

            // Deterministic hash (64 hex chars)
            var hash = SessionTokenHash.From(
                "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
            );

            return new GeneratedSessionToken(
                PlaintextToken: plaintext,
                Hash: hash
            );
        }
    }

    // ------------------------------------------------------------
    // 1. SUCCESSFUL COOKIE + HASH GENERATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Generates_token_hash_and_cookie()
    {
        var customerId = Guid.NewGuid();

        var ctx = new AuthCallbackContext("code")
        {
            CustomerId = customerId,
            Session = Session.Create(
                ownerId: CustomerId.From(customerId),
                tokenHash: SessionTokenHash.From(
                    "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc"
                ),
                createdAt: DateTimeOffset.UtcNow
            )
        };

        var step = new CreateSessionCookieStep(new FakeTokenService());

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        // TokenHash is set
        updated.TokenHash.Should().NotBeNull();
        updated.TokenHash!.Value.Should().Be(
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
        );

        // Session is NOT created here — it must already exist
        updated.Session.Should().NotBeNull();

        // Cookie is correct
        updated.SessionCookie.Should().NotBeNull();
        updated.SessionCookie!.Name.Should().Be("cfd.session");
        updated.SessionCookie!.Value.Should().Be(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        );
    }

    // ------------------------------------------------------------
    // 2. MISSING CUSTOMER ID THROWS
    // ------------------------------------------------------------
    [Fact]
    public async Task Missing_customerId_throws()
    {
        var ctx = new AuthCallbackContext("code");

        var step = new CreateSessionCookieStep(new FakeTokenService());

        Func<Task> act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
