using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeDogRepository : IDogRepository
{
    public List<Domain.Dogs.Dog> Dogs { get; } = [];

    public Task AddAsync(Domain.Dogs.Dog dog, CancellationToken cancellationToken = default)
    {
        Dogs.Add(dog);
        return Task.CompletedTask;
    }

    public Task<Domain.Dogs.Dog?> GetByIdAsync(DogId id, CancellationToken cancellationToken = default)
    {
        var dog = Dogs.FirstOrDefault(d => d.Id.Equals(id));
        return Task.FromResult(dog);
    }

    public Task DeleteAsync(Domain.Dogs.Dog dog, CancellationToken cancellationToken = default)
    {
        Dogs.Remove(dog);
        return Task.CompletedTask;
    }
}
