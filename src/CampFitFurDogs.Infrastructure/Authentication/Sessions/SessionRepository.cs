using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Authentication.Sessions;

public sealed class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash)
    {
        return await _db.Set<Session>()
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash);
    }

    public Task CreateAsync(Session session)
    {
        _db.Set<Session>().Add(session);
        return Task.CompletedTask;
    }

    public async Task RevokeAsync(SessionTokenHash tokenHash)
    {
        var session = await _db.Set<Session>()
            .SingleOrDefaultAsync(s => s.TokenHash == tokenHash);

        if (session is null)
            return;

        _db.Set<Session>().Remove(session);
    }

}
