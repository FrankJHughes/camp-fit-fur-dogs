using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public sealed class FakeGetDogByIdReader(List<Dog> _dogs) : IGetDogByIdReader
{
    public List<Dog> Dogs => _dogs;

    public Task<Dog?> ReadAsync(Guid dogId, CancellationToken cancellationToken = default)
    {
        var dog = _dogs.FirstOrDefault(d => d.Id.Value == dogId);
        return Task.FromResult(dog);
    }

}
