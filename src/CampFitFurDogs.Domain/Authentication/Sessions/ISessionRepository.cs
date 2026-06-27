using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Domain.Sessions;

[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ISessionRepository
{
    Task CreateAsync(Session session);

    Task RevokeAsync(SessionTokenHash tokenHash);

    Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash);
}
