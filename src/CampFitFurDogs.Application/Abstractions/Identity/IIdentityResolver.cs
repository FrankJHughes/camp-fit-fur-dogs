using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Abstractions.Identity;

public interface IIdentityResolver
{
    Task<Guid> ResolveAsync(
        AuthUser user,
        CancellationToken cancellationToken);
}
