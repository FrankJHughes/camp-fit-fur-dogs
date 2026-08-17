using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

/// <summary>
/// Represents the immutable builder‑request used to construct a
/// <see cref="CallbackSaveContext"/> during the save phase of the OIDC callback
/// pipeline.
/// <para>
/// This request contains the upstream identity information produced by the OIDC
/// callback builder, an optional redirect URL supplied by the client, and a
/// timestamp captured at the start of the application pipeline.
/// These values form the minimal set of inputs required to begin the save
/// operation, where the system resolves local identity, creates a session, and
/// determines the final redirect.
/// </para>
/// </summary>
/// <remarks>
/// This record inherits from <see cref="ImmutableContextBuilderRequestBase"/>,
/// ensuring that all inputs are immutable once supplied.
/// The request is intentionally minimal and focused on upstream‑provided values
/// and pipeline‑captured metadata, leaving all domain logic to downstream
/// components.
/// </remarks>
public sealed record CallbackSaveContextBuilderRequest : ImmutableContextBuilderRequestBase
{
    /// <summary>
    /// The normalized external identity information resolved by the OIDC
    /// callback builder.
    /// This includes subject identifiers, claims, UserInfo fields, and provider
    /// metadata extracted from the upstream identity provider.
    /// </summary>
    public required CallbackOidcContextBuilderResult External { get; init; }

    /// <summary>
    /// An optional redirect URL requested by the client.
    /// Downstream components may validate, sanitize, or override this value when
    /// determining the final redirect destination.
    /// </summary>
    public string? RequestedRedirectUrl { get; init; }

    /// <summary>
    /// The timestamp captured at the start of the application pipeline.
    /// <para>
    /// This value must be supplied by the caller using the application’s clock
    /// abstraction (e.g., <c>clock.UtcNow</c>).
    /// Capturing the timestamp externally ensures deterministic and testable
    /// time‑dependent behavior throughout the save operation.
    /// </para>
    /// </summary>
    public required DateTimeOffset Now { get; init; }
}
