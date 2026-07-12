using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Frank.Application.Abstractions.Sessions.GetSession;
using Frank.Application.Settings;
using Frank.Domain.Sessions;

namespace Frank.Infrastructure.EntityFrameworkCore.Sessions;

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

    public async Task<GetSessionResponse?> GetSessionAsync(
        string tokenHash, CancellationToken ct)
    {
        var session = await _db.Set<Session>()
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
