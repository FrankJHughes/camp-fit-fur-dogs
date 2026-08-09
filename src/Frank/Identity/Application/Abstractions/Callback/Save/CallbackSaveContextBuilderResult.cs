using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

/// <summary>
/// Represents the immutable result produced by the save‑phase builder in the
/// OIDC callback pipeline.
/// <para>
/// After upstream identity has been normalized and the application has resolved
/// or created a local user, this builder‑result captures the final application‑
/// level artifacts required to complete the callback flow:
/// </para>
/// <list type="bullet">
/// <item><description>The resolved internal user identifier</description></item>
/// <item><description>The newly created session identifier</description></item>
/// <item><description>A securely hashed representation of the session token</description></item>
/// <item><description>The cookie value that will be returned to the client</description></item>
/// </list>
/// <para>
/// These values are immutable once constructed and are used to populate the
/// <see cref="CallbackSaveContext"/> that drives cookie issuance and final
/// redirect behavior.
/// </para>
/// </summary>
/// <remarks>
/// This record inherits from <see cref="ImmutableContextBuilderResultBase"/>,
/// ensuring that all values are immutable and consistent with the Identity
/// subsystem’s deterministic pipeline model.
/// </remarks>
public sealed record CallbackSaveContextBuilderResult : ImmutableContextBuilderResultBase
{
    /// <summary>
    /// The internal user identifier resolved or created during the save phase.
    /// <para>
    /// This identifier represents the local application user associated with the
    /// upstream OIDC identity.
    /// It is guaranteed to be present once the save builder completes.
    /// </para>
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// The identifier of the newly created session associated with the user.
    /// <para>
    /// This value is used to correlate the session with audit logs, cookie
    /// generation, and downstream authorization flows.
    /// It is guaranteed to be present once the save builder completes.
    /// </para>
    /// </summary>
    public required Guid SessionId { get; init; }

    /// <summary>
    /// The hashed representation of the session token.
    /// <para>
    /// The raw token is never stored; only a secure hash is retained for
    /// validation and lookup.
    /// This value is required and always produced by the save builder.
    /// </para>
    /// </summary>
    public required string TokenHash { get; init; }

    /// <summary>
    /// The cookie value that will be returned to the client to establish the
    /// session.
    /// <para>
    /// This value is typically a signed or encrypted representation of the
    /// session token and is guaranteed to be present once the save builder
    /// completes.
    /// </para>
    /// </summary>
    public required string CookieValue { get; init; }
}
