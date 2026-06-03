using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.TestUtilities.Builders;

public class SessionBuilder
{
    private SessionTokenHash _tokenHash =
        SessionTokenHash.From(SessionFixtures.DefaultTokenHash);

    private CustomerId _ownerId =
        SessionFixtures.OwnerId;

    private DateTimeOffset _createdAt =
        SessionFixtures.CreatedAt;

    public SessionBuilder WithTokenHash(string hash)
    {
        _tokenHash = SessionTokenHash.From(hash);
        return this;
    }

    public SessionBuilder WithTokenHash(SessionTokenHash hash)
    {
        _tokenHash = hash;
        return this;
    }

    public SessionBuilder WithTokenHashFromFixture()
    {
        _tokenHash = SessionFixtures.TokenHash;
        return this;
    }

    public SessionBuilder WithOwner(CustomerId ownerId)
    {
        _ownerId = ownerId;
        return this;
    }

    public SessionBuilder WithOwnerFromFixture()
    {
        _ownerId = SessionFixtures.OwnerId;
        return this;
    }

    public SessionBuilder CreatedAt(DateTimeOffset timestamp)
    {
        _createdAt = timestamp;
        return this;
    }

    public SessionBuilder CreatedAtFromFixture()
    {
        _createdAt = SessionFixtures.CreatedAt;
        return this;
    }

    public Session Build()
        => Session.Create(
            tokenHash: _tokenHash,
            ownerId: _ownerId,
            createdAt: _createdAt
        );

    public SessionBuilder WithRandomTokenHash()
    {
        var random = Guid.NewGuid().ToString("N").PadLeft(64, 'a');
        _tokenHash = SessionTokenHash.From(random);
        return this;
    }

}
