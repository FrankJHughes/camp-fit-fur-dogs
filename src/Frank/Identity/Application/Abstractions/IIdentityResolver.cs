using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions;

public interface IIdentityResolver
{
    Task<Guid> ResolveAsync(
        CallbackOidcContextBuilderResult authCallbackResult,
        CancellationToken cancellationToken);
}
