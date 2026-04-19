using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeGetDogProfileReader : IGetDogProfileReader
{
    private readonly List<Dog> _dogs = [];

    public void Add(Dog dog) => _dogs.Add(dog);

    public Task<DogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct)
    {
        var dog = _dogs.FirstOrDefault(d => d.Id.Value == dogId);

        if (dog is null || dog.OwnerId.Value != ownerId)
            return Task.FromResult<DogProfileResponse?>(null);

        return Task.FromResult<DogProfileResponse?>(new DogProfileResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString()));
    }
}
