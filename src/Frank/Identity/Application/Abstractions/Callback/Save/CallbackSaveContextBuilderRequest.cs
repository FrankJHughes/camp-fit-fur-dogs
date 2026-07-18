using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

public sealed record CallbackSaveContextBuilderRequest : ImmutableContextBuilderRequestBase
{
    /// <summary>
    /// The external identity resolved by the protocol layer (Frank).
    /// </summary>
    public required CallbackOidcContextBuilderResult External { get; init; }

    /// <summary>
    /// Optional return URL requested by the client.
    /// </summary>
    public string? RequestedRedirectUrl { get; init; }

    /// <summary>
    /// Timestamp captured at the start of the application pipeline.
    /// </summary>
    public required DateTimeOffset Now { get; init; }
}
