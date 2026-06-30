using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Authentication.Callback.Steps;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;
using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

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
                Hash: SessionTokenHash.From(finalHash)
            );
        }

        public GeneratedSessionToken Generate() => Returned;
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
