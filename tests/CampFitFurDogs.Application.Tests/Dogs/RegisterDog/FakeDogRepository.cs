using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Dogs.RegisterDog;

public class FakeDogRepository : IDogRepository
{
    public List<Dog> Dogs { get; } = [];

    public Task AddAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        Dogs.Add(dog);
        return Task.CompletedTask;
    }
}
