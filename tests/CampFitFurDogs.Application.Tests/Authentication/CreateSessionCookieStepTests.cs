using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps

;
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
    // 1. SUCCESSFUL TOKEN HASH + COOKIE GENERATION
    // ------------------------------------------------------------
    [Fact]
    public async Task Generates_token_hash_and_cookie()
    {
        var customerId = Guid.NewGuid();

        var ctx = new AuthCallbackContext(
            Code: "code",
            CustomerId: customerId,
            TokenHash: SessionTokenHash.From(
                "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc"
            ),
            Session: null
        );

        var step = new IssueCookieStep(new FakeTokenService());

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        // TokenHash is overwritten with the generated hash
        updated.TokenHash.Should().NotBeNull();
        updated.TokenHash!.Value.Should().Be(
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
        );

        // Session is not created here
        updated.Session.Should().BeNull();

        // Cookie is correct
        updated.SessionCookie.Should().NotBeNull();
        updated.SessionCookie!.Name.Should().Be("cfd.session");
        updated.SessionCookie!.Value.Should().Be(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        );
    }

    // ------------------------------------------------------------
    // 2. DOES NOT REQUIRE EXISTING SESSION
    // ------------------------------------------------------------
    [Fact]
    public async Task Does_not_require_existing_session()
    {
        var ctx = new AuthCallbackContext(
            Code: "code",
            CustomerId: Guid.NewGuid()
        );

        var step = new IssueCookieStep(new FakeTokenService());

        var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().NotThrowAsync<InvalidOperationException>();
    }
}
