namespace Frank.Domain.Sessions;

public interface ISessionRepository
{
    Task CreateAsync(Session session, CancellationToken cancellationToken);

    Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken);

    Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken);
}
