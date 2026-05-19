using FluentAssertions;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using CampFitFurDogs.Application.Dogs.ListDogsByOwner;
using CampFitFurDogs.Application.Tests.Fakes;

using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;

using CampFitFurDogs.Domain.Customers;

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

        var dog1 = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var dog2 = new DogBuilder()
            .WithOwner(ownerId)
            .WithName("Maple")
            .WithBreed("Beagle")
            .BornOn(new DateOnly(2023, 3, 10))
            .WithSex(Domain.Dogs.Sex.Male)
            .Build();

        _reader.Add(dog1);
        _reader.Add(dog2);

        var query = new ListDogsByOwnerQuery(ownerId.Value);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Dogs.Should().HaveCount(2);
        result.Dogs.Should().Contain(d =>
            d.Name == DogFixtures.DefaultName && d.Breed == DogFixtures.DefaultBreed);
        result.Dogs.Should().Contain(d =>
            d.Name == "Maple" && d.Breed == "Beagle");
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

        var dogA = new DogBuilder()
            .WithOwner(ownerA)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        var dogB = new DogBuilder()
            .WithOwner(ownerB)
            .WithName("Maple")
            .WithBreed("Beagle")
            .BornOn(new DateOnly(2023, 3, 10))
            .WithSex(Domain.Dogs.Sex.Male)
            .Build();

        _reader.Add(dogA);
        _reader.Add(dogB);

        var query = new ListDogsByOwnerQuery(ownerA.Value);

        var result = await _handler.HandleAsync(query, CancellationToken.None);

        result.Dogs.Should().HaveCount(1);
        result.Dogs[0].Name.Should().Be(DogFixtures.DefaultName);
    }
}
