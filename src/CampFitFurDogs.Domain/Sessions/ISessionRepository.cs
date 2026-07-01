using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Domain.Sessions;

[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ISessionRepository
{
    Task CreateAsync(Session session, CancellationToken cancellationToken);

    Task RevokeAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken);

    Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash, CancellationToken cancellationToken);
}
