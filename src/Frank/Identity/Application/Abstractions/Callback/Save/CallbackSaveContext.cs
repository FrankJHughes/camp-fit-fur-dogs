using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

/// <summary>
/// Represents the immutable context used during the “save” phase of the OIDC
/// callback pipeline.
/// <para>
/// After the upstream OIDC provider has been processed and identity information
/// has been normalized, this context drives the application‑level logic for:
/// </para>
/// <list type="bullet">
/// <item><description>Resolving or creating a local user</description></item>
/// <item><description>Creating a session and token</description></item>
/// <item><description>Generating a cookie value</description></item>
/// <item><description>Determining the final redirect URL</description></item>
/// </list>
/// <para>
/// All values in this context are immutable once constructed, ensuring
/// deterministic and replayable behavior throughout the Identity pipeline.
/// </para>
/// </summary>
/// <remarks>
/// This record inherits from <see cref="ImmutableContextBase"/>, which enforces
/// immutability and supports consistent context handling across the Identity
/// subsystem.
/// The context is intentionally structured to separate upstream identity data
/// (<see cref="External"/>) from application‑level state (user resolution,
/// session creation, cookie generation).
/// </remarks>
public sealed record CallbackSaveContext : ImmutableContextBase
{
    //
    // Immutable inputs
    //

    /// <summary>
    /// The normalized identity information extracted from the upstream OIDC
    /// provider.
    /// This result contains subject identifiers, claims, UserInfo fields, and
    /// provider metadata produced by the OIDC callback builder.
    /// </summary>
    public required CallbackOidcContextBuilderResult External { get; init; }

    /// <summary>
    /// The timestamp representing the moment the save operation is executed.
    /// <para>
    /// This value must be supplied by the caller using the application’s clock
    /// abstraction (e.g., <c>clock.UtcNow</c>).
    /// Capturing the timestamp externally ensures deterministic and testable
    /// time‑dependent behavior throughout the save pipeline.
    /// </para>
    /// </summary>
    public required DateTimeOffset Now { get; init; }

    /// <summary>
    /// An optional redirect URL requested by the frontend or upstream caller.
    /// <para>
    /// This value may influence the final redirect decision but is validated and
    /// sanitized by downstream components to ensure safe navigation.
    /// </para>
    /// </summary>
    public string? RequestedRedirectUrl { get; init; }

    //
    // Domain identity resolution
    //

    /// <summary>
    /// The internal user identifier resolved or created during the save phase.
    /// <para>
    /// If the upstream identity maps to an existing user, this value is set to
    /// that user’s identifier.
    /// If no user exists, downstream components may create one and populate this
    /// field accordingly.
    /// </para>
    /// </summary>
    public Guid? UserId { get; init; }

    //
    // Session creation
    //

    /// <summary>
    /// The identifier of the newly created session, if session creation succeeds.
    /// <para>
    /// This value is used to correlate the session with audit logs, cookie
    /// generation, and downstream authorization flows.
    /// </para>
    /// </summary>
    public Guid? SessionId { get; init; }

    /// <summary>
    /// The hashed representation of the session token, if generated.
    /// <para>
    /// The raw token is never stored; only a secure hash is retained for
    /// validation purposes.
    /// </para>
    /// </summary>
    public string? TokenHash { get; init; }

    //
    // Cookie generation
    //

    /// <summary>
    /// The cookie value that will be returned to the client to establish the
    /// session.
    /// This value is typically a signed or encrypted representation of the
    /// session token.
    /// </summary>
    public string? CookieValue { get; init; }

    //
    // Final redirect
    //

    /// <summary>
    /// The final redirect URL that the client should be sent to after the save
    /// operation completes.
    /// <para>
    /// This value may be derived from the requested redirect URL, provider
    /// metadata, or application‑level routing rules.
    /// </para>
    /// </summary>
    public string? RedirectUrl { get; init; }
}
