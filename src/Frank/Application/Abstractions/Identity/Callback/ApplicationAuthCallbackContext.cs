using Frank.Abstractions.Identity.Callback;
using Frank.Abstractions.ImmutableContext;

namespace Frank.Application.Abstractions.Identity.Callback;

public sealed record ApplicationAuthCallbackContext : ImmutableContextBase
{
    //
    // Immutable inputs
    //
    public required FrankAuthCallbackResult External { get; init; }
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
