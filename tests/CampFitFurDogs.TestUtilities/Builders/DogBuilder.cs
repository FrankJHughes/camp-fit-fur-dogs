using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Fixtures;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

namespace CampFitFurDogs.TestUtilities.Builders;

public class DogBuilder
{
    private CustomerId _owner = CustomerId.New();
    private string _name = DogFixtures.DefaultName;
    private string _breed = DogFixtures.DefaultBreed;
    private DateOnly _dob = DogFixtures.Dob;
    private Sex _sex = DogFixtures.Sex;

    public DogBuilder WithOwner(CustomerId owner)
    {
        _owner = owner;
        return this;
    }

    public DogBuilder WithName(string name)
    {
        _name = name;   // <-- allow invalid values
        return this;
    }

    public DogBuilder WithBreed(string breed)
    {
        _breed = breed; // <-- allow invalid values
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

    public Dog Build()
        => Dog.Create(
            _owner,
            DogName.Create(_name),   // <-- validation happens here
            Breed.Create(_breed),
            _dob,
            _sex);

    public RegisterDogCommand BuildApiRequest()
        => new RegisterDogCommand(
            OwnerId: _owner.Value,
            Name: _name,             // <-- raw string, no validation
            Breed: _breed,
            DateOfBirth: _dob,
            Sex: _sex.ToString());
}
