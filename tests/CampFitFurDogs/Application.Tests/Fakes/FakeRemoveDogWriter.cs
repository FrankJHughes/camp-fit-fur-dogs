using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Fakes;

public sealed class FakeRemoveDogWriter(List<Dog> Dogs) : IRemoveDogWriter
{
    public Task WriteAsync(Guid dogId, CancellationToken cancellationToken = default)
    {
        var dog = Dogs.SingleOrDefault(d => d.Id.Value == dogId)
            ?? throw new InvalidOperationException($"Dog {dogId} not found");
        Dogs.Remove(dog);
        return Task.CompletedTask;
    }
}
