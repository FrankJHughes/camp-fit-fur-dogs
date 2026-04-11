using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Domain.Dogs;

public sealed class Dog : AggregateRoot<DogId>
{
    public CustomerId OwnerId { get; private set; } = default!;
    public DogName Name { get; private set; } = default!;
    public Breed Breed { get; private set; } = default!;
    public DateOnly DateOfBirth { get; private set; }
    public Sex Sex { get; private set; }

    private Dog() { }

    private Dog(DogId id, CustomerId ownerId, DogName name, Breed breed, DateOnly dateOfBirth, Sex sex)
        : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        Breed = breed;
        DateOfBirth = dateOfBirth;
        Sex = sex;
    }

    public static Dog Create(CustomerId ownerId, DogName name, Breed breed, DateOnly dateOfBirth, Sex sex)
    {
        return new Dog(DogId.New(), ownerId, name, breed, dateOfBirth, sex);
    }
}
