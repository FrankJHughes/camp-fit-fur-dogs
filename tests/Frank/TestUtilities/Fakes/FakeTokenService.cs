using Frank.Core.Application.Abstractions.Authentication;
using Frank.Identity.Application.Abstractions;
using Frank.Identity.Domain.Sessions;

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
            HashedToken: hash
        );
    }

    public SessionTokenHash Hash(string plaintextToken)
    {
        throw new NotImplementedException();
    }
}
