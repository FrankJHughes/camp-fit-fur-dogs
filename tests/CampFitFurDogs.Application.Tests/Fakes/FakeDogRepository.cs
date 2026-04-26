using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeDogRepository : IDogRepository
{
    public List<Dog> Dogs { get; } = [];

    public Task AddAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        Dogs.Add(dog);
        return Task.CompletedTask;
    }

    public Task<Dog?> GetByIdAsync(DogId id, CancellationToken cancellationToken = default)
    {
        var dog = Dogs.FirstOrDefault(d => d.Id.Equals(id));
        return Task.FromResult(dog);
    }

    public Task DeleteAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        Dogs.Remove(dog);
        return Task.CompletedTask;
    }
}
