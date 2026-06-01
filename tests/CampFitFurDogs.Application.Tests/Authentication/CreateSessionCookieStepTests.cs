using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

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
            var plaintext = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

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
    public async Task Generates_token_hash_and_cookie_and_result()
    {
        var customerId = Guid.NewGuid();

        var ctx = new AuthCallbackContext("code")
        {
            CustomerId = customerId
        };

        var step = new CreateSessionCookieStep(new FakeTokenService());

        await step.ExecuteAsync(ctx, CancellationToken.None);

        // TokenHash is set
        ctx.TokenHash.Should().NotBeNull();
        ctx.TokenHash!.Value.Should().Be(
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
        );

        // Result is set
        ctx.Result.Should().NotBeNull();
        ctx.Result!.CustomerId.Should().Be(CustomerId.From(customerId));

        // Cookie is correct
        ctx.Result.Cookie.Name.Should().Be("cfd.session");
        ctx.Result.Cookie.Value.Should().Be(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        );

        ctx.Result.RedirectUrl.Should().Be("");
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
