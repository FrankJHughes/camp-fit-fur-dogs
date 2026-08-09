using Frank.Core.Application.Abstractions.Clock;
using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Identity.EntityFrameworkCore.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

/// <summary>
/// EF Core implementation of <see cref="IGetSessionReader"/> responsible for
/// retrieving and evaluating <see cref="Session"/> aggregates.
/// <para>
/// This reader is part of the vertical slice for session retrieval. It loads
/// session data from the database, evaluates expiration based on configured TTL,
/// and returns a <see cref="GetSessionResponse"/> containing all relevant
/// session metadata.
/// </para>
/// <para>
/// The reader uses <see cref="AsNoTracking"/> because session retrieval is
/// read‑only and does not require EF Core change tracking.
/// </para>
/// </summary>
public sealed class GetSessionReader : IGetSessionReader
{
    private readonly FrankIdentityDbContext _db;
    private readonly TimeSpan _ttl;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new <see cref="GetSessionReader"/> using the provided
    /// DbContext, session TTL configuration, and clock abstraction.
    /// </summary>
    /// <param name="db">The EF Core DbContext used to query session data.</param>
    /// <param name="sessionOptions">
    /// Provides access to <see cref="SessionSettings"/>, including the session TTL.
    /// </param>
    /// <param name="clock">
    /// Clock abstraction used to evaluate the current time in a testable manner.
    /// </param>
    public GetSessionReader(
        FrankIdentityDbContext db,
        IOptionsMonitor<SessionSettings> sessionOptions,
        IClock clock)
    {
        _db = db;
        _ttl = sessionOptions.CurrentValue.Ttl;
        _clock = clock;
    }

    /// <summary>
    /// Retrieves a session by its token hash and evaluates its expiration.
    /// </summary>
    /// <param name="tokenHash">The hashed session token.</param>
    /// <param name="ct">A cancellation token for the operation.</param>
    /// <returns>
    /// A <see cref="GetSessionResponse"/> if the session exists; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Expiration is computed as <c>CreatedAt + TTL</c>.
    /// Evaluation time is provided by <see cref="IClock.UtcNow"/>.
    /// </remarks>
    public async Task<GetSessionResponse?> ReadAsync(
        string tokenHash, CancellationToken ct)
    {
        var session = await _db.Set<Session>()
            .AsNoTracking()
            .Where(s => s.TokenHash == SessionTokenHash.From(tokenHash))
            .SingleOrDefaultAsync(ct);

        if (session is null)
            return null;

        return new GetSessionResponse(
            Id: session.Id.Value,
            OwnerId: session.OwnerId.Value,
            CreatedAt: session.CreatedAt,
            RevokedAt: session.RevokedAt,
            ExpiresAt: session.CreatedAt + _ttl,
            EvaluatedAt: _clock.UtcNow);
    }
}
