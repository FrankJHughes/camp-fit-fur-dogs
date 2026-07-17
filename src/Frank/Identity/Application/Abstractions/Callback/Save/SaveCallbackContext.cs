using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

public sealed record SaveCallbackContext : ImmutableContextBase
{
    //
    // Immutable inputs
    //
    public required OidcCallbackContextBuilderResult External { get; init; }
    public required DateTimeOffset Now { get; init; }
    public string? RequestedRedirectUrl { get; init; }

    //
    // Domain identity resolution
    //
    public Guid? UserId { get; init; }

    //
    // Session creation
    //
    public Guid? SessionId { get; init; }
    public string? TokenHash { get; init; }

    //
    // Cookie generation
    //
    public string? CookieValue { get; init; }

    //
    // Final redirect
    //
    public string? RedirectUrl { get; init; }
}
