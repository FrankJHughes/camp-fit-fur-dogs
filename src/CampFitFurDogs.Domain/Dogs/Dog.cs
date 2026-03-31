namespace CampFitFurDogs.Domain.Dogs;

using CampFitFurDogs.Domain.Guardians;
using CampFitFurDogs.SharedKernel;

public sealed class Dog : AggregateRoot<DogId>
{
    public string Name { get; private set; }
    public string Breed { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public GuardianId GuardianId { get; private set; }

    private Dog(DogId id, string name, string breed, DateOnly dateOfBirth, GuardianId guardianId)
        : base(id)
    {
        Name = name;
        Breed = breed;
        DateOfBirth = dateOfBirth;
        GuardianId = guardianId;
    }

    public static Dog Create(string name, string breed, DateOnly dateOfBirth, GuardianId guardianId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(breed);
        ArgumentNullException.ThrowIfNull(guardianId);

        return new Dog(DogId.New(), name, breed, dateOfBirth, guardianId);
    }
}
