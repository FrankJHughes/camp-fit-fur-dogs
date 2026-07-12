using Frank.Application.Abstractions.Authentication;
using Frank.Application.Abstractions.Identity;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Application.Identity.Callback.Steps;
using Frank.Domain.Sessions;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace Frank.Application.Tests.Authentication.Callback.Steps;

public sealed class BuildCookieStepTests
{
    private const string ValidHash =
        "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    private sealed class FakeTokenService : ISessionTokenService
    {
        public GeneratedSessionToken Returned { get; }

        public FakeTokenService(
            string plaintext = "plaintext-token",
            string? hash = null)
        {
            var finalHash = hash ?? ValidHash;

            Returned = new GeneratedSessionToken(
                PlaintextToken: plaintext,
                HashedToken: SessionTokenHash.From(finalHash)
            );
        }

        public GeneratedSessionToken Generate() => Returned;

        public SessionTokenHash Hash(string plaintextToken)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task ExecuteAsync_SetsTokenHash_AndCookieValue()
    {
        var tokens = new FakeTokenService(hash: ValidHash);
        var step = new BuildCookieStep(tokens);

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        result.TokenHash.Should().Be(ValidHash);
        result.CookieValue.Should().NotBeNull();
    }
}
