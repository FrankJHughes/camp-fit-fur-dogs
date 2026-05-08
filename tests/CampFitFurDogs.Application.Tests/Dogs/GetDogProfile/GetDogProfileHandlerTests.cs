using FluentAssertions;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Dogs.GetDogProfile;

public class GetDogProfileHandlerTests
{
    private readonly FakeGetDogProfileReader _reader = new();
    private readonly GetDogProfileHandler _handler;

    public GetDogProfileHandlerTests()
    {
        _handler = new GetDogProfileHandler(_reader);
    }

    [Fact]
    public async Task Handle_DogExistsAndOwnedByCustomer_ReturnsProfile()
    {
        var ownerId = CustomerId.From(Guid.NewGuid());
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        _reader.Add(dog);

        var query = new GetDogProfileQuery(dog.Id.Value, ownerId.Value);

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
        var query = new GetDogProfileQuery(Guid.NewGuid(), Guid.NewGuid());
        var result = await _handler.HandleAsync(query, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DogExistsButNotOwnedByCustomer_ReturnsNull()
    {
        var ownerA = Guid.NewGuid();
        var ownerB = Guid.NewGuid();

        var dog = Dog.Create(
            CustomerId.From(ownerA),
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        _reader.Add(dog);

        var handler = new GetDogProfileHandler(_reader);
        var query = new GetDogProfileQuery(dog.Id.Value, ownerB);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
