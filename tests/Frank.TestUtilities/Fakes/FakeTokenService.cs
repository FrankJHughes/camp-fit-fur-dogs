using Frank.Application.Abstractions.Identity;
using Frank.Domain.Sessions;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeTokenService : ISessionTokenService
{
    public GeneratedSessionToken Generate()
    {
        // Deterministic plaintext token (64 hex chars)
        const string plaintext =
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
