using Frank.Abstractions.Authentication.Callback;

namespace Frank.Abstractions.Identity;

public interface IIdentityResolver
{
    Task<Guid> ResolveAsync(
        FrankAuthCallbackResult authCallbackResult,
        CancellationToken cancellationToken);
}
