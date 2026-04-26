using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using FluentAssertions;

namespace CampFitFurDogs.Application.Tests.Dogs.EditDogProfile;

public class EditDogProfileHandlerTests
{
    [Fact]
    public async Task Handle_WhenDogExistsAndOwnerMatches_UpdatesDogAndCommits()
    {
        // Arrange
        var ownerId = CustomerId.New();
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Poodle"),
            new DateOnly(2022, 1, 1),
            Sex.Male);

        var repo = new FakeDogRepository();
        await repo.AddAsync(dog);
        var uow = new FakeUnitOfWork();
        var handler = new EditDogProfileHandler(repo, uow);

        var command = new EditDogProfileCommand(
            DogId: dog.Id.Value,
            OwnerId: ownerId.Value,
            Name: "Waffles",
            Breed: "Labrador",
            DateOfBirth: new DateOnly(2021, 6, 15),
            Sex: "Female");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await repo.GetByIdAsync(dog.Id);
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
        var repo = new FakeDogRepository();
        var uow = new FakeUnitOfWork();
        var handler = new EditDogProfileHandler(repo, uow);

        var command = new EditDogProfileCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            Name: "Waffles",
            Breed: "Labrador",
            DateOfBirth: new DateOnly(2021, 6, 15),
            Sex: "Female");

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOwnerDoesNotMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var ownerId = CustomerId.New();
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Poodle"),
            new DateOnly(2022, 1, 1),
            Sex.Male);

        var repo = new FakeDogRepository();
        await repo.AddAsync(dog);
        var uow = new FakeUnitOfWork();
        var handler = new EditDogProfileHandler(repo, uow);

        var command = new EditDogProfileCommand(
            DogId: dog.Id.Value,
            OwnerId: Guid.NewGuid(),   // ← different owner
            Name: "Waffles",
            Breed: "Labrador",
            DateOfBirth: new DateOnly(2021, 6, 15),
            Sex: "Female");

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }
}
