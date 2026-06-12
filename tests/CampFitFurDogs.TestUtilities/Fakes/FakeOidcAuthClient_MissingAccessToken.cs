using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Errors;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeOidcAuthClient_MissingAccessToken : IAuthClient
{
    public Task<AuthToken> ExchangeAsync(string authorizationCode, CancellationToken ct)
        => throw new ExternalAuthProviderException("Missing access token");

    public Task<AuthUser> GetUserAsync(AuthToken token, CancellationToken ct)
        => throw new InvalidOperationException("Should not be called");
}
