using FluentAssertions;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Application.Dogs.ListDogsByOwner;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Dogs.ListDogsByOwner;

public class ListDogsByOwnerHandlerTests
{
    private readonly FakeListDogsByOwnerReader _reader = new();
    private readonly ListDogsByOwnerHandler _handler;

    public ListDogsByOwnerHandlerTests()
    {
        _handler = new ListDogsByOwnerHandler(_reader);
    }

    [Fact]
    public async Task Handle_OwnerHasMultipleDogs_ReturnsAllDogs()
    {
        var ownerId = CustomerId.From(Guid.NewGuid());

        var dog1 = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        var dog2 = Dog.Create(
            ownerId,
            DogName.Create("Maple"),
            Breed.Create("Beagle"),
            new DateOnly(2023, 3, 10),
            Sex.Male);

        _reader.Add(dog1);
        _reader.Add(dog2);

        var query = new ListDogsByOwnerQuery(ownerId.Value);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Dogs.Should().HaveCount(2);
        result.Dogs.Should().Contain(d => d.Name == "Biscuit" && d.Breed == "Golden Retriever");
        result.Dogs.Should().Contain(d => d.Name == "Maple" && d.Breed == "Beagle");
    }

    [Fact]
    public async Task Handle_OwnerHasNoDogs_ReturnsEmptyList()
    {
        var query = new ListDogsByOwnerQuery(Guid.NewGuid());

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Dogs.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_OnlyReturnsDogsBelongingToOwner()
    {
        var ownerA = CustomerId.From(Guid.NewGuid());
        var ownerB = CustomerId.From(Guid.NewGuid());

        var dogA = Dog.Create(
            ownerA,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        var dogB = Dog.Create(
            ownerB,
            DogName.Create("Maple"),
            Breed.Create("Beagle"),
            new DateOnly(2023, 3, 10),
            Sex.Male);

        _reader.Add(dogA);
        _reader.Add(dogB);

        var query = new ListDogsByOwnerQuery(ownerA.Value);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Dogs.Should().HaveCount(1);
        result.Dogs[0].Name.Should().Be("Biscuit");
    }
}
