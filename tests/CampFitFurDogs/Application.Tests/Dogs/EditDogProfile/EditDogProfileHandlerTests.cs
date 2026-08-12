using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Application.Dogs.EditDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Tests.Dogs.EditDog;

public class EditDogCommandHandlerTests
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

        var registerWriter = new FakeRegisterDogWriter(dogs);
        // await registerWriter.WriteAsync(dog);
        var reader = new FakeGetDogByIdReader(dogs);
        var editWriter = new FakeEditDogWriter(dogs);
        var uow = new FakeAppUnitOfWork();
        var handler = new EditDogCommandHandler(editWriter, uow);

        var command = new EditDogCommand(
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

}
