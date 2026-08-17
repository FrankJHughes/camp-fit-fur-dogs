using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.TestUtilities.Fixtures;
using Frank.Identity.Application.Abstractions.Users;

namespace CampFitFurDogs.Application.Tests.Dogs.RegisterDog;

public class RegisterDogCommandValidatorTests
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

    private readonly RegisterDogCommandValidator _validator;

    public RegisterDogCommandValidatorTests()
    {
        var currentUser = new FakeCurrentUser(Guid.NewGuid());
        _validator = new RegisterDogCommandValidator(currentUser);
    }

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(), // mismatched owner is fine; name is the failure we care about
            Name: "",
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString()
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
