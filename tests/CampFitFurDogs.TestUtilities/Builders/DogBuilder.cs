using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.TestUtilities.Builders;

public sealed class DogBuilder
{
    private CustomerId _owner = CustomerId.New();
    private string _name = DogFixtures.DefaultName;
    private string _breed = DogFixtures.DefaultBreed;
    private DateOnly _dob = DogFixtures.Dob;
    private Sex _sex = DogFixtures.Sex;

    // ------------------------------------------------------------
    // Fluent configuration
    // ------------------------------------------------------------

    public DogBuilder WithOwner(CustomerId owner)
    {
        _owner = owner;
        return this;
    }

    public DogBuilder WithName(string name)
    {
        _name = name; // allow invalid raw values for API tests
        return this;
    }

    public DogBuilder WithBreed(string breed)
    {
        _breed = breed; // allow invalid raw values for API tests
        return this;
    }

    public DogBuilder BornOn(DateOnly dob)
    {
        _dob = dob;
        return this;
    }

    public DogBuilder WithSex(Sex sex)
    {
        _sex = sex;
        return this;
    }

    // ------------------------------------------------------------
    // Domain aggregate creation (validates via value objects)
    // ------------------------------------------------------------

    public Dog Build()
        => Dog.Create(
            _owner,
            DogName.Create(_name),   // domain validation
            Breed.Create(_breed),    // domain validation
            _dob,
            _sex);

    // ------------------------------------------------------------
    // API request creation (raw values, no validation)
    // ------------------------------------------------------------

    public RegisterDogCommand BuildApiRequest()
        => new(
            OwnerId: _owner.Value,
            Name: _name,
            Breed: _breed,
            DateOfBirth: _dob,
            Sex: _sex.ToString());
}
