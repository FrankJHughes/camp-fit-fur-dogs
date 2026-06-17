using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Domain.Authentication.Sessions;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]
public interface ISessionRepository
{
    Task CreateAsync(Session session);

    Task RevokeAsync(SessionTokenHash tokenHash);

    Task<Session?> GetByTokenHashAsync(SessionTokenHash tokenHash);
}
