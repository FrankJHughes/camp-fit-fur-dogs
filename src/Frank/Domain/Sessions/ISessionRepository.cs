namespace Frank.Domain.Sessions;

public interface ISessionRepository
{
    Task CreateAsync(Session session);

    Task RevokeAsync(SessionTokenHash tokenHash);

    Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash);
}
