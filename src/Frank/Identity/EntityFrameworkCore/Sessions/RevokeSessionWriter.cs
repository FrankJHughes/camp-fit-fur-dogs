using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

/// <summary>
/// EF Core implementation of <see cref="IRevokeSessionWriter"/> responsible for
/// revoking an existing <see cref="Session"/> aggregate.
/// <para>
/// This writer is part of the vertical slice for session revocation. It loads
/// the session by its token hash, invokes the domain‑level revoke behavior, and
/// relies on EF Core change tracking to persist the modification.
/// </para>
/// <para>
/// The writer does not call <c>SaveChangesAsync</c>; the application layer
/// coordinates transactional boundaries through its unit‑of‑work.
/// </para>
/// </summary>
public sealed class RevokeSessionWriter : IRevokeSessionWriter
{
    private readonly FrankIdentityDbContext _db;

    /// <summary>
    /// Initializes a new <see cref="RevokeSessionWriter"/> using the provided
    /// <see cref="FrankIdentityDbContext"/>.
    /// </summary>
    /// <param name="db">The EF Core DbContext used to load and track session entities.</param>
    public RevokeSessionWriter(FrankIdentityDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Revokes a session identified by its <see cref="SessionTokenHash"/>.
    /// <para>
    /// If the session exists, the domain method <see cref="Session.Revoke"/> is
    /// invoked with the current UTC timestamp. EF Core tracks the change, and
    /// the caller is responsible for committing the transaction.
    /// </para>
    /// </summary>
    /// <param name="tokenHash">The hashed session token identifying the session to revoke.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A completed task.</returns>
    public async Task WriteAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken)
    {
        var session = await _db.Set<Session>()
            .SingleOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);

        if (session is null)
            return;

        // Domain behavior
        session.Revoke(DateTimeOffset.UtcNow);

        // EF will track the change; SaveChanges is handled by the unit of work
    }
}
