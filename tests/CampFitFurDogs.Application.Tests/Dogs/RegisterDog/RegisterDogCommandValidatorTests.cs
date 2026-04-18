using FluentAssertions;
using FluentValidation;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;

namespace CampFitFurDogs.Application.Tests.Dogs.RegisterDog;

public class RegisterDogCommandValidatorTests
{
    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var validator = new RegisterDogCommandValidator();

        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: "",
            Breed: "Labrador",
            DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            Sex: "Male"
        );

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}
