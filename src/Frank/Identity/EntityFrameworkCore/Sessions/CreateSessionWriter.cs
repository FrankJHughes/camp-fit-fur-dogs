using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.DbContexts;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

/// <summary>
/// EF Core implementation of <see cref="ICreateSessionWriter"/> responsible for
/// persisting new <see cref="Session"/> aggregates.
/// <para>
/// This writer is part of the vertical slice for session creation. It performs
/// the persistence step by attaching the <see cref="Session"/> entity to the
/// <see cref="FrankIdentityDbContext"/>.
/// </para>
/// <para>
/// The writer does not call <c>SaveChangesAsync</c>; the application layer
/// coordinates unit‑of‑work boundaries. This ensures consistency with CQRS and
/// transactional command handling.
/// </para>
/// </summary>
public sealed class CreateSessionWriter : ICreateSessionWriter
{
    private readonly FrankIdentityDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSessionWriter"/> using
    /// the provided <see cref="FrankIdentityDbContext"/>.
    /// </summary>
    /// <param name="db">The EF Core DbContext used to persist session data.</param>
    public CreateSessionWriter(FrankIdentityDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Persists a newly created <see cref="Session"/> aggregate by adding it to
    /// the EF Core change tracker.
    /// <para>
    /// The caller is responsible for committing the transaction via
    /// <c>SaveChangesAsync</c>.
    /// </para>
    /// </summary>
    /// <param name="session">The session aggregate to persist.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A completed task.</returns>
    public Task WriteAsync(Session session, CancellationToken cancellationToken)
    {
        _db.Set<Session>().Add(session);
        return Task.CompletedTask;
    }
}
