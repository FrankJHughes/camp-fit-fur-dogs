using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public sealed class FakeRegisterDogWriter : IRegisterDogWriter
{
    public List<Dog> Dogs { get; init; } = default!;

    public FakeRegisterDogWriter()
    {
        Dogs = [];
    }
    public FakeRegisterDogWriter(List<Dog> dogs)
    {
        Dogs = dogs;
    }
    public Task WriteAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        Dogs.Add(dog);
        return Task.CompletedTask;
    }
}
