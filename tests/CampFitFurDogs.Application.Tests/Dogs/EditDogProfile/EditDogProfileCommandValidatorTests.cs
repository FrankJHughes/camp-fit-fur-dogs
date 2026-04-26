using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Dogs.EditDogProfile;
using FluentAssertions;

namespace CampFitFurDogs.Application.Tests.Dogs.EditDogProfile;

public class EditDogProfileCommandValidatorTests
{
    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        var validator = new EditDogProfileCommandValidator();
        var command = new EditDogProfileCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            Name: "",
            Breed: "Labrador",
            DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            Sex: "Male");

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_fail_when_breed_is_empty()
    {
        var validator = new EditDogProfileCommandValidator();
        var command = new EditDogProfileCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            Name: "Biscuit",
            Breed: "",
            DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            Sex: "Male");

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_fail_when_dogId_is_empty()
    {
        var validator = new EditDogProfileCommandValidator();
        var command = new EditDogProfileCommand(
            DogId: Guid.Empty,
            OwnerId: Guid.NewGuid(),
            Name: "Biscuit",
            Breed: "Labrador",
            DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            Sex: "Male");

        var result = validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }
}
