using CampFitFurDogs.Domain.Sessions;
using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Sessions;

public sealed class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Domain.Sessions.Session?> GetByTokenHashAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken)
    {
        return await _db.Set<Session>()
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);
    }

    public Task CreateAsync(Domain.Sessions.Session session, CancellationToken cancellationToken)
    {
        _db.Set<Domain.Sessions.Session>().Add(session);
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
