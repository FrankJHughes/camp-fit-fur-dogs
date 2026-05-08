using Microsoft.Extensions.DependencyInjection;

using SharedKernel.DependencyInjection;

namespace CampFitFurDogs.Domain.Dogs;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true, MaxRegistrationCount = 1)]

public interface IDogRepository
{
    Task AddAsync(Dog dog, CancellationToken cancellationToken = default);
    Task<Dog?> GetByIdAsync(DogId id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Dog dog, CancellationToken cancellationToken = default);
}
