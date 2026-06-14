using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Errors;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeOidcAuthClient_UserInfoFailure : IAuthClient
{
    public Task<AuthToken> ExchangeAsync(string authorizationCode, CancellationToken ct)
        => Task.FromResult(new AuthToken("fake-access-token"));

    public Task<AuthUser> GetUserAsync(AuthToken token, CancellationToken ct)
        => throw new ExternalAuthProviderException("Failed to retrieve user profile");
}
