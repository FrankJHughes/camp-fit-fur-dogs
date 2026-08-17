using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using CampFitFurDogs.Application.Dogs.GetDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Tests.Dogs.GetDog;

public class GetDogQueryHandlerTests
{
    private readonly FakeGetDogReader _reader = new();
    private readonly GetDogQueryHandler _handler;

    public GetDogQueryHandlerTests()
    {
        _handler = new GetDogQueryHandler(_reader);
    }

    [Fact]
    public async Task Handle_DogExistsAndOwnedByUser_ReturnsProfile()
    {
        var ownerId = UserId.From(Guid.NewGuid());
        var dog = Domain.Dogs.Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        _reader.Add(dog);

        var query = new GetDogQuery(dog.Id.Value, ownerId.Value);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        Assert.Equal(dog.Id.Value, result!.Id);
        Assert.Equal(ownerId.Value, result.OwnerId);
        Assert.Equal("Biscuit", result.Name);
        Assert.Equal("Golden Retriever", result.Breed);
        Assert.Equal(new DateOnly(2022, 6, 15), result.DateOfBirth);
        Assert.Equal("Female", result.Sex);
    }

    [Fact]
    public async Task Handle_DogNotFound_ResultShouldBeNull()
    {
        var query = new GetDogQuery(Guid.NewGuid(), Guid.NewGuid());
        var result = await _handler.HandleAsync(query, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DogExistsButNotOwnedByUser_ReturnsNull()
    {
        var ownerA = Guid.NewGuid();
        var ownerB = Guid.NewGuid();

        var dog = Domain.Dogs.Dog.Create(
            UserId.From(ownerA),
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        _reader.Add(dog);

        var handler = new GetDogQueryHandler(_reader);
        var query = new GetDogQuery(dog.Id.Value, ownerB);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
