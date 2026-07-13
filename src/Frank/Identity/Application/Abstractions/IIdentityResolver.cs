using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Abstractions;

public interface IIdentityResolver
{
    Task<Guid> ResolveAsync(
        OidcCallbackContextBuilderResult authCallbackResult,
        CancellationToken cancellationToken);
}
