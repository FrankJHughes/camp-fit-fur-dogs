using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Application.Dogs.RemoveDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Tests.Dogs.RemoveDog;

public class RemoveDogHandlerTests
{
    [Fact]
    public async Task Handle_WhenDogExistsAndOwnerMatches_RemovesDogAndCommits()
    {
        var ownerId = UserId.New();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var dogs = new List<Dog>
        {
            dog
        };

        var reader = new FakeGetDogByIdReader(dogs);
        var writer = new FakeRemoveDogWriter(dogs);

        var uow = new FakeAppUnitOfWork();
        var handler = new RemoveDogHandler(reader, writer, uow);

        var command = new RemoveDogCommand(
            DogId: dog.Id.Value,
            OwnerId: ownerId.Value);

        await handler.HandleAsync(command, CancellationToken.None);

        var removed = await reader.ReadAsync(dog.Id.Value);
        removed.Should().BeNull();

        uow.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDogNotFound_ThrowsInvalidOperationException()
    {
        var dogs = new List<Dog>();
        var reader = new FakeGetDogByIdReader(dogs);
        var writer = new FakeRemoveDogWriter(dogs);
        var uow = new FakeAppUnitOfWork();

        var handler = new RemoveDogHandler(reader, writer, uow);

        var command = new RemoveDogCommand(
            DogId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid());

        var act = () => handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenOwnerDoesNotMatch_ThrowsInvalidOperationException()
    {
        var ownerId = UserId.New();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var dogs = new List<Dog> { dog };
        var reader = new FakeGetDogByIdReader(dogs);
        var writer = new FakeRemoveDogWriter(dogs);

        var uow = new FakeAppUnitOfWork();
        var handler = new RemoveDogHandler(reader, writer, uow);

        var command = new RemoveDogCommand(
            DogId: dog.Id.Value,
            OwnerId: Guid.NewGuid()); // wrong owner

        var act = () => handler.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        uow.Committed.Should().BeFalse();
    }
}
