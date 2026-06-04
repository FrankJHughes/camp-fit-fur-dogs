using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Tests.Dogs;

public class DogRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public DogRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<CustomerId> SeedOwnerAsync()
    {
        await using var ctx = _fixture.CreateContext();
        var repo = new CustomerRepository(ctx);

        var customer = new CustomerBuilder()
            .WithFirstName(CustomerFixtures.First.Value)
            .WithLastName(CustomerFixtures.Last.Value)
            .WithEmail($"infra-{Guid.NewGuid()}@example.com")
            .WithPhone(CustomerFixtures.Phone.Value)
            .WithPassword(CustomerFixtures.Hash.Value)
            .Build();

        await repo.AddAsync(customer, CancellationToken.None);
        await ctx.SaveChangesAsync();

        return customer.Id;
    }

    [Fact]
    public async Task AddAsync_persists_dog_with_correct_mapping()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _fixture.CreateContext();
        var repo = new DogRepository(ctx);

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        await repo.AddAsync(dog, CancellationToken.None);
        await ctx.SaveChangesAsync();

        await using var readCtx = _fixture.CreateContext();
        var persisted = await readCtx.Set<Dog>()
            .AsNoTracking()
            .SingleAsync(d => d.Id == dog.Id);

        persisted.OwnerId.Should().Be(ownerId);
        persisted.Name.Should().Be(DogName.Create(DogFixtures.DefaultName));
        persisted.Breed.Should().Be(Breed.Create(DogFixtures.DefaultBreed));
        persisted.DateOfBirth.Should().Be(DogFixtures.Dob);
        persisted.Sex.Should().Be(DogFixtures.Sex);
    }

    [Fact]
    public async Task AddAsync_with_nonexistent_owner_throws_on_save()
    {
        await using var ctx = _fixture.CreateContext();
        var repo = new DogRepository(ctx);

        var fakeOwnerId = CustomerId.From(Guid.NewGuid());

        var dog = new DogBuilder()
            .WithOwner(fakeOwnerId)
            .WithName("Ghost")
            .WithBreed("Husky")
            .BornOn(new DateOnly(2023, 1, 1))
            .WithSex(Sex.Male)
            .Build();

        await repo.AddAsync(dog, CancellationToken.None);

        Func<Task> act = () => ctx.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task AddAsync_persists_multiple_dogs_for_same_owner()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _fixture.CreateContext();
        var repo = new DogRepository(ctx);

        var dog1 = new DogBuilder()
            .WithOwner(ownerId)
            .WithName("Maple")
            .WithBreed("Poodle")
            .BornOn(new DateOnly(2023, 3, 10))
            .WithSex(Sex.Female)
            .Build();

        var dog2 = new DogBuilder()
            .WithOwner(ownerId)
            .WithName("Bandit")
            .WithBreed("Beagle")
            .BornOn(new DateOnly(2021, 8, 5))
            .WithSex(Sex.Male)
            .Build();

        await repo.AddAsync(dog1, CancellationToken.None);
        await repo.AddAsync(dog2, CancellationToken.None);
        await ctx.SaveChangesAsync();

        await using var readCtx = _fixture.CreateContext();
        var ownerDogs = await readCtx.Set<Dog>()
            .AsNoTracking()
            .Where(d => d.OwnerId.Equals(ownerId))
            .ToListAsync();

        ownerDogs.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_existing_dog_returns_full_profile()
    {
        var ownerId = await SeedOwnerAsync();

        await using var writeCtx = _fixture.CreateContext();
        var writeRepo = new DogRepository(writeCtx);

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName("Biscuit")
            .WithBreed("Golden Retriever")
            .BornOn(new DateOnly(2022, 6, 15))
            .WithSex(Sex.Female)
            .Build();

        await writeRepo.AddAsync(dog, CancellationToken.None);
        await writeCtx.SaveChangesAsync();

        await using var readCtx = _fixture.CreateContext();
        var reader = new GetDogProfileReader(readCtx);

        var result = await reader.GetDogProfileAsync(
            dog.Id.Value,
            ownerId.Value,
            CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(dog.Id.Value);
        result.OwnerId.Should().Be(ownerId.Value);
        result.Name.Should().Be("Biscuit");
        result.Breed.Should().Be("Golden Retriever");
        result.DateOfBirth.Should().Be(new DateOnly(2022, 6, 15));
        result.Sex.Should().Be(Sex.Female.ToString());
    }

    [Fact]
    public async Task GetByIdAsync_nonexistent_id_returns_null()
    {
        await using var readCtx = _fixture.CreateContext();
        var reader = new GetDogProfileReader(readCtx);

        var result = await reader.GetDogProfileAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        result.Should().BeNull();
    }
}
