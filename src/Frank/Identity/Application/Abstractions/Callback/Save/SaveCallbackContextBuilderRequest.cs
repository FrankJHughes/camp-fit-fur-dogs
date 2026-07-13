using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

public sealed record SaveCallbackContextBuilderRequest : ImmutableContextBuilderRequestBase
{
    /// <summary>
    /// The external identity resolved by the protocol layer (Frank).
    /// </summary>
    public required OidcCallbackContextBuilderResult External { get; init; }

    /// <summary>
    /// Optional return URL requested by the client.
    /// </summary>
    public string? RequestedRedirectUrl { get; init; }

    /// <summary>
    /// Timestamp captured at the start of the application pipeline.
    /// </summary>
    public required DateTimeOffset Now { get; init; }
}
