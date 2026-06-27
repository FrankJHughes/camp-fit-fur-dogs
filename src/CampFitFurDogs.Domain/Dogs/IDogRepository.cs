using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Domain.Dogs;

[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]

public interface IDogRepository
{
    Task AddAsync(Dog dog, CancellationToken cancellationToken = default);
    Task<Dog?> GetByIdAsync(DogId id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Dog dog, CancellationToken cancellationToken = default);
}
