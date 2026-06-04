using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;

namespace CampFitFurDogs.Infrastructure.Tests.Dogs;

public class ListDogsByOwnerReaderTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public ListDogsByOwnerReaderTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<CustomerId> SeedCustomerAsync(AppDbContext ctx, string uniqueTag)
    {
        var customer = new CustomerBuilder()
            .WithFirstName(CustomerFixtures.First.Value)
            .WithLastName(CustomerFixtures.Last.Value)
            .WithEmail($"{uniqueTag}@example.com")
            .WithPhone(CustomerFixtures.Phone.Value)
            .WithPassword(PasswordFixtures.Plain)
            .Build();

        await new CustomerRepository(ctx).AddAsync(customer, CancellationToken.None);
        return customer.Id;
    }

    private async Task<Dog> SeedDogAsync(
        AppDbContext ctx, CustomerId ownerId, string name, string breed)
    {
        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(name)
            .WithBreed(breed)
            .BornOn(new DateOnly(2022, 6, 15))
            .WithSex(Sex.Female)
            .Build();

        await new DogRepository(ctx).AddAsync(dog, CancellationToken.None);
        return dog;
    }

    [Fact]
    public async Task ListDogsByOwnerAsync_OwnerHasMultipleDogs_ReturnsAll()
    {
        await using var seedCtx = _fixture.CreateContext();
        var ownerId = await SeedCustomerAsync(seedCtx, $"list-multi-{Guid.NewGuid()}");

        await SeedDogAsync(seedCtx, ownerId, "Biscuit", "Golden Retriever");
        await SeedDogAsync(seedCtx, ownerId, "Maple", "Beagle");
        await seedCtx.SaveChangesAsync();

        await using var readCtx = _fixture.CreateContext();
        var reader = new ListDogsByOwnerReader(readCtx);

        var result = await reader.ListDogsByOwnerAsync(ownerId.Value, CancellationToken.None);

        result.Dogs.Should().HaveCount(2);
        result.Dogs.Should().Contain(d => d.Name == "Biscuit" && d.Breed == "Golden Retriever");
        result.Dogs.Should().Contain(d => d.Name == "Maple" && d.Breed == "Beagle");
    }

    [Fact]
    public async Task ListDogsByOwnerAsync_OwnerHasNoDogs_ReturnsEmptyList()
    {
        await using var ctx = _fixture.CreateContext();
        var reader = new ListDogsByOwnerReader(ctx);

        var result = await reader.ListDogsByOwnerAsync(Guid.NewGuid(), CancellationToken.None);

        result.Dogs.Should().BeEmpty();
    }

    [Fact]
    public async Task ListDogsByOwnerAsync_OnlyReturnsDogsBelongingToOwner()
    {
        await using var seedCtx = _fixture.CreateContext();

        var ownerA = await SeedCustomerAsync(seedCtx, $"list-a-{Guid.NewGuid()}");
        var ownerB = await SeedCustomerAsync(seedCtx, $"list-b-{Guid.NewGuid()}");

        await SeedDogAsync(seedCtx, ownerA, "Biscuit", "Golden Retriever");
        await SeedDogAsync(seedCtx, ownerB, "Rex", "German Shepherd");

        await seedCtx.SaveChangesAsync();

        await using var readCtx = _fixture.CreateContext();
        var reader = new ListDogsByOwnerReader(readCtx);

        var result = await reader.ListDogsByOwnerAsync(ownerA.Value, CancellationToken.None);

        result.Dogs.Should().HaveCount(1);
        result.Dogs[0].Name.Should().Be("Biscuit");
    }
}
