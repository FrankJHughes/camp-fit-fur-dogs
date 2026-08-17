using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace Frank.Identity.Application.Abstractions.Sessions.RevokeSession;

/// <summary>
/// Represents a command used to revoke an existing session based on its token hash.
/// <para>
/// This command is part of the Identity subsystem’s session‑revocation pipeline.
/// It is issued when the application needs to invalidate an authenticated session
/// so that future authentication attempts using the associated token are rejected.
/// </para>
/// <para>
/// The command carries only the secure, non‑reversible token hash.
/// The raw session token is never persisted or exposed.
/// </para>
/// </summary>
/// <remarks>
/// The command handler is responsible for:
/// <list type="bullet">
/// <item><description>Looking up the session using the token hash</description></item>
/// <item><description>Marking the session as revoked</description></item>
/// <item><description>Persisting the revocation timestamp</description></item>
/// <item><description>Ensuring deterministic evaluation using the <c>IClock</c> abstraction</description></item>
/// </list>
/// </remarks>
public sealed record RevokeSessionCommand(string TokenHash) : ICommand;
