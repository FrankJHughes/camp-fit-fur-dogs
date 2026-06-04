using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Application.Tests.Dogs.RegisterDog;

public class RegisterDogCommandValidatorTests
{
    private readonly RegisterDogCommandValidator _validator = new();

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: "",
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString()
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
