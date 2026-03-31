namespace CampFitFurDogs.Domain.Dogs;

using CampFitFurDogs.Domain.Guardians;
using CampFitFurDogs.SharedKernel;

public sealed class Dog : AggregateRoot<DogId>
{
    public string Name { get; private set; } = default!;
    public string Breed { get; private set; } = default!;
    public DateOnly DateOfBirth { get; private set; }
    public GuardianId GuardianId { get; private set; } = default!;

    private Dog(DogId id) : base(id) { }

    public static Dog Create(string name, string breed, DateOnly dateOfBirth, GuardianId guardianId)
        => throw new NotImplementedException();
}
