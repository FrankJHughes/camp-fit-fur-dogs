using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using CampFitFurDogs.TestUtilities.Builders;
using FluentAssertions;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Frank.Identity.EntityFrameworkCore.Users;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;

namespace CampFitFurDogs.Infrastructure.Tests.Dogs;

public class ListDogsByOwnerReaderTests :
    IClassFixture<PostgresFixture<FrankIdentityDbContext>>,
    IClassFixture<PostgresFixture<AppDbContext>>
{
    private readonly PostgresFixture<FrankIdentityDbContext> _identity;
    private readonly PostgresFixture<AppDbContext> _dogs;

    public ListDogsByOwnerReaderTests(
        PostgresFixture<FrankIdentityDbContext> identity,
        PostgresFixture<AppDbContext> dogs)
    {
        _identity = identity;
        _dogs = dogs;
    }

    private async Task<UserId> SeedUserAsync(string uniqueTag)
    {
        await using var identityCtx = _identity.CreateContext();

        var user = new UserBuilder()
            .WithFirstName(UserFixtures.First.Value)
            .WithLastName(UserFixtures.Last.Value)
            .WithEmail($"{uniqueTag}@example.com")
            .WithPhone(UserFixtures.Phone.Value)
            .Build();

        await new UserRepository(identityCtx).AddAsync(user, CancellationToken.None);
        await identityCtx.SaveChangesAsync();

        return user.Id;
    }

    private async Task<Domain.Dogs.Dog> SeedDogAsync(UserId ownerId, string name, string breed)
    {
        await using var dogsCtx = _dogs.CreateContext();

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(name)
            .WithBreed(breed)
            .BornOn(new DateOnly(2022, 6, 15))
            .WithSex(Sex.Female)
            .Build();

        await new DogRepository(dogsCtx).AddAsync(dog, CancellationToken.None);
        await dogsCtx.SaveChangesAsync();

        return dog;
    }

    [Fact]
    public async Task ListDogsByOwnerAsync_OwnerHasMultipleDogs_ReturnsAll()
    {
        var ownerId = await SeedUserAsync($"list-multi-{Guid.NewGuid()}");

        await SeedDogAsync(ownerId, "Biscuit", "Golden Retriever");
        await SeedDogAsync(ownerId, "Maple", "Beagle");

        await using var readCtx = _dogs.CreateContext();
        var reader = new ListDogsByOwnerReader(readCtx);

        var result = await reader.ListDogsByOwnerAsync(ownerId.Value, CancellationToken.None);

        result.Dogs.Should().HaveCount(2);
        result.Dogs.Should().Contain(d => d.Name == "Biscuit" && d.Breed == "Golden Retriever");
        result.Dogs.Should().Contain(d => d.Name == "Maple" && d.Breed == "Beagle");
    }

    [Fact]
    public async Task ListDogsByOwnerAsync_OwnerHasNoDogs_ReturnsEmptyList()
    {
        await using var readCtx = _dogs.CreateContext();
        var reader = new ListDogsByOwnerReader(readCtx);

        var result = await reader.ListDogsByOwnerAsync(Guid.NewGuid(), CancellationToken.None);

        result.Dogs.Should().BeEmpty();
    }

    [Fact]
    public async Task ListDogsByOwnerAsync_OnlyReturnsDogsBelongingToOwner()
    {
        var ownerA = await SeedUserAsync($"list-a-{Guid.NewGuid()}");
        var ownerB = await SeedUserAsync($"list-b-{Guid.NewGuid()}");

        await SeedDogAsync(ownerA, "Biscuit", "Golden Retriever");
        await SeedDogAsync(ownerB, "Rex", "German Shepherd");

        await using var readCtx = _dogs.CreateContext();
        var reader = new ListDogsByOwnerReader(readCtx);

        var result = await reader.ListDogsByOwnerAsync(ownerA.Value, CancellationToken.None);

        result.Dogs.Should().HaveCount(1);
        result.Dogs[0].Name.Should().Be("Biscuit");
    }
}
