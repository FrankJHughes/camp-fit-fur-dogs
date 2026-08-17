using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

/// <summary>
/// Represents the immutable builder‑request used to construct a
/// <see cref="CallbackOidcContext"/> during an OIDC callback flow.
/// <para>
/// Builder‑request types capture the minimal set of inputs required to begin
/// constructing an immutable context.
/// In this case, the OIDC authorization <see cref="Code"/> is the only required
/// protocol input needed to initiate the callback pipeline.
/// </para>
/// </summary>
/// <remarks>
/// This record inherits from <see cref="ImmutableContextBuilderRequestBase"/>,
/// ensuring that all builder inputs are treated as immutable once supplied.
/// Additional fields may be added over time as the callback pipeline evolves,
/// but the builder‑request remains intentionally minimal and focused on
/// upstream‑provided values.
/// </remarks>
public record CallbackOidcContextBuilderRequest : ImmutableContextBuilderRequestBase
{
    /// <summary>
    /// The authorization code returned by the upstream OIDC provider during the
    /// callback phase.
    /// This value is required to perform the token exchange that produces the
    /// access token, ID token, and associated identity information.
    /// </summary>
    public required string Code { get; init; }
}
