using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Users;

public interface IUserResolver
{
    Task<Guid> ResolveAsync(
        CallbackOidcContextBuilderResult authCallbackResult,
        CancellationToken cancellationToken);
}
