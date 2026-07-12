using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Frank.Domain.Sessions;

namespace Frank.TestUtilities.Fakes;

public sealed class FakeSessionRepository : ISessionRepository
{
    public List<Session> CreatedSessions { get; } = [];
    public List<SessionTokenHash> RevokedHashes { get; } = [];
    public Session? SessionToReturn { get; set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task CreateAsync(Session session, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        CreatedSessions.Add(session);
        return Task.CompletedTask;
    }

    public Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        RevokedHashes.Add(tokenHash);
        return Task.CompletedTask;
    }

    public Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash, CancellationToken ct)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(SessionToReturn);
    }
}
