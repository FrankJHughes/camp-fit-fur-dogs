using CampFitFurDogs.Application.Abstractions.Dog.RemoveDog;
using CampFitFurDogs.Application.Dogs.RemoveDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Application.Tests.Dogs.RemoveDog;

public class RemoveDogHandlerTests
{
    [Fact]
    public async Task Handle_WhenDogExistsAndOwnerMatches_RemovesDogAndCommits()
    {
        // Arrange
        var ownerId = CustomerId.New();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var repo = new FakeDogRepository();
        await repo.AddAsync(dog);

        var uow = new FakeUnitOfWork();
        var handler = new RemoveDogHandler(repo, uow);

        var command = new RemoveDogCommand(
            DogId: dog.Id.Value,
            OwnerId: ownerId.Value);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

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
        var act = () => handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOwnerDoesNotMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var ownerId = CustomerId.New();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var repo = new FakeDogRepository();
        await repo.AddAsync(dog);

        var uow = new FakeUnitOfWork();
        var handler = new RemoveDogHandler(repo, uow);

        var command = new RemoveDogCommand(
            DogId: dog.Id.Value,
            OwnerId: Guid.NewGuid()); // wrong owner

        // Act
        var act = () => handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }
}
