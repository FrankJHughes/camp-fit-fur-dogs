using System.Security.Cryptography;
using System.Text;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;

namespace CampFitFurDogs.Application.Authentication;

public sealed class SessionTokenService : ISessionTokenService
{
    public GeneratedSessionToken Generate()
    {
        // 1. Generate secure random plaintext token (256-bit)
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToHexString(tokenBytes).ToLowerInvariant();

        // 2. Hash for DB storage
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var tokenHash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return new GeneratedSessionToken(
            PlaintextToken: token,
            Hash: SessionTokenHash.From(tokenHash)
        );
    }
}
