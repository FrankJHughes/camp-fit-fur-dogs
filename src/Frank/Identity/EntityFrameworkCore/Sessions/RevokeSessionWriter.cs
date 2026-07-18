using Frank.Identity.Application.Abstractions.Sessions.RevokeSession;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

public sealed class RevokeSessionWriter : IRevokeSessionWriter
{
    private readonly FrankIdentityDbContext _db;

    public RevokeSessionWriter(FrankIdentityDbContext db)
    {
        _db = db;
    }

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
