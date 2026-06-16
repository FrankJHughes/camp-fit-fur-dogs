using Frank.Abstractions.Authentication.Callback;

namespace CampFitFurDogs.Application.Abstractions.Identity;

public interface IIdentityResolver
{
    Task<Guid> ResolveAsync(
        FrankAuthCallbackResult authCallbackResult,
        CancellationToken cancellationToken);
}
