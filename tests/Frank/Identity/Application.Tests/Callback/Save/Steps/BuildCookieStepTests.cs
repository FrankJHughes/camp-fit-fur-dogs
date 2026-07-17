using Frank.Core.Application.Abstractions.Authentication;
using Frank.Identity.Application.Abstractions;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Callback.Save.Steps;
using Frank.Identity.Domain.Sessions;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace Frank.Identity.Application.Tests.Callback.Save.Steps;

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

        var ctx = new SaveCallbackContext
        {
            External = FakeOidcCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        result.TokenHash.Should().Be(ValidHash);
        result.CookieValue.Should().NotBeNull();
    }
}
