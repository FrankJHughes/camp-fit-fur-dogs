using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

public sealed class SessionRepository : ISessionRepository
{
    private readonly FrankIdentityDbContext _db;

    public SessionRepository(FrankIdentityDbContext db)
    {
        _db = db;
    }

    public async Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken)
    {
        return await _db.Set<Session>()
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);
    }

    public Task CreateAsync(Session session, CancellationToken cancellationToken)
    {
        _db.Set<Session>().Add(session);
        return Task.CompletedTask;
    }

    public async Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken)
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
