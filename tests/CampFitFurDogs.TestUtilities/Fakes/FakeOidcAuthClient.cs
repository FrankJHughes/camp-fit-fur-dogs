using CampFitFurDogs.Application.Abstractions.Authentication;

public sealed class FakeOidcAuthClient : IAuthClient
{
    public Task<AuthToken> ExchangeAsync(string authorizationCode, CancellationToken ct)
    {
        return Task.FromResult(new AuthToken("fake-access-token"));
    }

    public Task<AuthUser> GetUserAsync(AuthToken token, CancellationToken ct)
    {
        var newGuid = Guid.NewGuid();

        // IMPORTANT: sub must be in "provider|id" format
        return Task.FromResult(new AuthUser(
            ExternalId: $"auth0|{newGuid}",
            GivenName: "Test",
            FamilyName: "User",
            Email: $"test.user.{newGuid}@example.com"
        ));
    }
}
