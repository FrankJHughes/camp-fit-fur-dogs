using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeGetDogReader : IGetDogReader
{
    private readonly List<Domain.Dogs.Dog> _dogs = [];

    public void Add(Domain.Dogs.Dog dog) => _dogs.Add(dog);

    public Task<GetDogResponse?> ReadAsync(
        Guid dogId, Guid ownerId, CancellationToken ct)
    {
        var dog = _dogs.FirstOrDefault(d => d.Id.Value == dogId);

        if (dog is null || dog.OwnerId.Value != ownerId)
            return Task.FromResult<GetDogResponse?>(null);

        return Task.FromResult<GetDogResponse?>(new GetDogResponse(
            dog.Id.Value,
            dog.OwnerId.Value,
            dog.Name.Value,
            dog.Breed.Value,
            dog.DateOfBirth,
            dog.Sex.ToString()));
    }
}
