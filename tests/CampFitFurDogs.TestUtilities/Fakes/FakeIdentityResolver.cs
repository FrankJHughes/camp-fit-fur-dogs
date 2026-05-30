using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Identity;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeIdentityResolver : IIdentityResolver
{
    public Guid Result { get; set; }

    // Captured inputs for assertions
    public string? LastExternalId { get; private set; }
    public string? LastFirstName { get; private set; }
    public string? LastLastName { get; private set; }
    public string? LastEmail { get; private set; }

    public Task<Guid> ResolveAsync(AuthUser user, CancellationToken ct)
    {
        LastExternalId = user.ExternalId;
        LastFirstName = user.GivenName;
        LastLastName = user.FamilyName;
        LastEmail = user.Email;

        return Task.FromResult(Result);
    }
}
