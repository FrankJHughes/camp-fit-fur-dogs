using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Domain.Dogs;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Tests.Fakes;

public sealed class FakeEditDogProfileWriter : IEditDogProfileWriter
{
    public List<Dog> Dogs { get; init; } = default!;

    public FakeEditDogProfileWriter()
    {
        Dogs = [];
    }
    public FakeEditDogProfileWriter(List<Dog> dogs)
    {
        Dogs = dogs;
    }
    public Task WriteAsync(UserId ownerId, DogId id, DogName name, Breed breed, DateOnly dateOfBirth, Sex sex, CancellationToken cancellationToken)
    {
        Dogs.SingleOrDefault(d => d.Id == id && d.OwnerId == ownerId)?.Update(name, breed, dateOfBirth, sex);
        return Task.CompletedTask;
    }
}
