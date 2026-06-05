using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Dogs.EditDogProfile;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Application.Tests.Dogs.EditDogProfile;

public class EditDogProfileCommandValidatorTests
{
    private readonly EditDogProfileCommandValidator _validator = new();

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var command = new EditDogProfileCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            Name: "",
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_fail_when_breed_is_empty()
    {
        var command = new EditDogProfileCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            Name: DogFixtures.DefaultName,
            Breed: "",
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_fail_when_dogId_is_empty()
    {
        var command = new EditDogProfileCommand(
            DogId: Guid.Empty,
            OwnerId: Guid.NewGuid(),
            Name: DogFixtures.DefaultName,
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
