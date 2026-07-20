using Frank.Identity.Application.Abstractions.Sessions.GetSession;
using Frank.Identity.Application.Settings;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Frank.Identity.EntityFrameworkCore.Sessions;

public sealed class GetSessionReader : IGetSessionReader
{
    private readonly FrankIdentityDbContext _db;
    private readonly TimeSpan _ttl;

    public GetSessionReader(
        FrankIdentityDbContext db,
        IOptionsMonitor<SessionSettings> sessionOptions)
    {
        _db = db;
        _ttl = sessionOptions.CurrentValue.Ttl;
    }

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
            ExpiresAt: session.CreatedAt + _ttl);
    }
}
