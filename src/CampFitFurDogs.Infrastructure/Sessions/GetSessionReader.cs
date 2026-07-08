using Microsoft.EntityFrameworkCore;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Application.Abstractions.Sessions.GetSession;
using CampFitFurDogs.Domain.Sessions;
using Microsoft.Extensions.Options;
using CampFitFurDogs.Application.Settings;

namespace CampFitFurDogs.Infrastructure.Sessions;

public sealed class GetSessionReader : IGetSessionReader
{
    private readonly AppDbContext _db;
    private readonly TimeSpan _ttl;

    public GetSessionReader(
        AppDbContext db,
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
