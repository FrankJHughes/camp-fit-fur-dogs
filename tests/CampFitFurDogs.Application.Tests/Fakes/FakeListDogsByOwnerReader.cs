using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public class FakeListDogsByOwnerReader : IListDogsByOwnerReader
{
    private readonly List<Dog> _dogs = [];

    public void Add(Dog dog) => _dogs.Add(dog);

    public Task<ListDogsByOwnerResponse> ListDogsByOwnerAsync(
        Guid ownerId, CancellationToken ct)
    {
        var summaries = _dogs
            .Where(d => d.OwnerId.Value == ownerId)
            .Select(d => new DogSummary(d.Id.Value, d.Name.Value, d.Breed.Value))
            .ToList();

        return Task.FromResult(new ListDogsByOwnerResponse(summaries));
    }
}
