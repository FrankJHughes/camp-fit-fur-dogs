namespace Frank.Identity.Domain.Sessions;

public interface ISessionRepository
{
    Task CreateAsync(Session session, CancellationToken cancellationToken);

    Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken);
}
