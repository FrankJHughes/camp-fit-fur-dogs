using CampFitFurDogs.Domain.Sessions;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeSessionRepository : ISessionRepository
{
    public List<Session> CreatedSessions { get; } = new();
    public List<SessionTokenHash> RevokedHashes { get; } = new();
    public Session? SessionToReturn { get; set; }
    public Exception? ExceptionToThrow { get; set; }

    public Task CreateAsync(Session session)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        CreatedSessions.Add(session);
        return Task.CompletedTask;
    }

    public Task RevokeAsync(SessionTokenHash tokenHash)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        RevokedHashes.Add(tokenHash);
        return Task.CompletedTask;
    }

    public Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash)
    {
        if (ExceptionToThrow is not null)
            throw ExceptionToThrow;

        return Task.FromResult(SessionToReturn);
    }
}
