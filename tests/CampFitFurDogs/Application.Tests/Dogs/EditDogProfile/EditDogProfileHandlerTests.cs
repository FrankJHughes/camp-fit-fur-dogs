using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Tests.Dogs.EditDogProfile;

public class EditDogProfileHandlerTests
{
    [Fact]
    public async Task Handle_WhenDogExistsAndOwnerMatches_UpdatesDogAndCommits()
    {
        // Arrange
        var ownerId = UserId.New();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var dogs = new List<Dog> { dog };

        var writer = new FakeRegisterDogWriter(dogs);
        await writer.WriteAsync(dog);
        var reader = new FakeGetDogByIdReader(dogs);
        var uow = new FakeAppUnitOfWork();
        var handler = new EditDogProfileHandler(reader, uow);

        var command = new EditDogProfileCommand(
            DogId: dog.Id.Value,
            OwnerId: ownerId.Value,
            Name: "Waffles",
            Breed: "Labrador",
            DateOfBirth: new DateOnly(2021, 6, 15),
            Sex: "Female");

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        var updated = await reader.ReadAsync(dog.Id.Value);

        updated!.Name.Value.Should().Be("Waffles");
        updated.Breed.Value.Should().Be("Labrador");
        updated.DateOfBirth.Should().Be(new DateOnly(2021, 6, 15));
        updated.Sex.Should().Be(Sex.Female);

        uow.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDogNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var reader = new FakeGetDogByIdReader([]);
        var uow = new FakeAppUnitOfWork();
        var handler = new EditDogProfileHandler(reader, uow);

        var command = new EditDogProfileCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            Name: DogFixtures.DefaultName,
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString());

        // Act
        var act = () => handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOwnerDoesNotMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var ownerId = UserId.New();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var dogs = new List<Dog>();

        var reader = new FakeGetDogByIdReader(dogs);
        var writer = new FakeRegisterDogWriter(dogs);
        var uow = new FakeAppUnitOfWork();

        await writer.WriteAsync(dog);

        var handler = new EditDogProfileHandler(reader, uow);

        var command = new EditDogProfileCommand(
            DogId: dog.Id.Value,
            OwnerId: Guid.NewGuid(), // wrong owner
            Name: "Waffles",
            Breed: "Labrador",
            DateOfBirth: new DateOnly(2021, 6, 15),
            Sex: "Female");

        // Act
        var act = () => handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }
}
