using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Application.Dogs.EditDog;
using CampFitFurDogs.TestUtilities.Fixtures;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Tests.Dogs.EditDog;

public class EditDogCommandValidatorTests
{
    private sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid? Id { get; init; }

        public bool IsAuthenticated => throw new NotImplementedException();

        public string? Name => throw new NotImplementedException();

        public FakeCurrentUser(Guid id)
        {
            Id = id;
        }
    }

    private readonly EditDogCommandValidator _validator;

    public EditDogCommandValidatorTests()
    {
        var currentUser = new FakeCurrentUser(Guid.NewGuid());
        _validator = new EditDogCommandValidator(currentUser);
    }

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var command = new EditDogCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(), // mismatched owner is fine; name is the failure we care about
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
        var command = new EditDogCommand(
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
        var command = new EditDogCommand(
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
