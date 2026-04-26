using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Application.Dogs.RemoveDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using FluentAssertions;

namespace CampFitFurDogs.Application.Tests.Dogs.RemoveDog;

public class RemoveDogHandlerTests
{
    [Fact]
    public async Task Handle_WhenDogExistsAndOwnerMatches_RemovesDogAndCommits()
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
        var handler = new RemoveDogHandler(repo, uow);

        var command = new RemoveDogCommand(
            DogId: dog.Id.Value,
            OwnerId: ownerId.Value);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var removed = await repo.GetByIdAsync(dog.Id);
        removed.Should().BeNull();
        uow.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDogNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var repo = new FakeDogRepository();
        var uow = new FakeUnitOfWork();
        var handler = new RemoveDogHandler(repo, uow);

        var command = new RemoveDogCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid());

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
        var handler = new RemoveDogHandler(repo, uow);

        var command = new RemoveDogCommand(
            DogId: dog.Id.Value,
            OwnerId: Guid.NewGuid());

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }
}
