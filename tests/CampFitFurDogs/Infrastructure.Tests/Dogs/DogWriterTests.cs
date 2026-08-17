using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Identity.EntityFrameworkCore.Users;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Infrastructure.Tests.Dogs;

public class DogWriterTests :
    IClassFixture<PostgresFixture<FrankIdentityDbContext>>,
    IClassFixture<PostgresFixture<AppDbContext>>
{
    private readonly PostgresFixture<FrankIdentityDbContext> _identity;
    private readonly PostgresFixture<AppDbContext> _dogs;

    public DogWriterTests(
        PostgresFixture<FrankIdentityDbContext> identity,
        PostgresFixture<AppDbContext> dogs)
    {
        _identity = identity;
        _dogs = dogs;
    }

    private async Task<UserId> SeedOwnerAsync()
    {
        await using var ctx = _identity.CreateContext();
        var writer = new CreateUserWriter(ctx);

        var user = new UserBuilder()
            .WithFirstName(UserFixtures.First.Value)
            .WithLastName(UserFixtures.Last.Value)
            .WithEmail($"infra-{Guid.NewGuid()}@example.com")
            .WithPhone(UserFixtures.Phone.Value)
            .Build();

        await writer.WriteAsync(user, CancellationToken.None);
        await ctx.SaveChangesAsync();

        return user.Id;
    }

    [Fact]
    public async Task WriteAsync_persists_dog_with_correct_mapping()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _dogs.CreateContext();
        var writer = new RegisterDogWriter(ctx);

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName(DogFixtures.DefaultName)
            .WithBreed(DogFixtures.DefaultBreed)
            .BornOn(DogFixtures.Dob)
            .WithSex(DogFixtures.Sex)
            .Build();

        await writer.WriteAsync(dog, CancellationToken.None);
        await ctx.SaveChangesAsync();

        await using var readCtx = _dogs.CreateContext();
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
    public async Task WriteAsync_persists_multiple_dogs_for_same_owner()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _dogs.CreateContext();
        var writer = new RegisterDogWriter(ctx);

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

        await writer.WriteAsync(dog1, CancellationToken.None);
        await writer.WriteAsync(dog2, CancellationToken.None);
        await ctx.SaveChangesAsync();

        await using var readCtx = _dogs.CreateContext();
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

        await using var writeCtx = _dogs.CreateContext();
        var writer = new RegisterDogWriter(writeCtx);

        var dog = new DogBuilder()
            .WithOwner(ownerId)
            .WithName("Biscuit")
            .WithBreed("Golden Retriever")
            .BornOn(new DateOnly(2022, 6, 15))
            .WithSex(Sex.Female)
            .Build();

        await writer.WriteAsync(dog, CancellationToken.None);
        await writeCtx.SaveChangesAsync();

        await using var readCtx = _dogs.CreateContext();
        var reader = new GetDogReader(readCtx);

        var result = await reader.ReadAsync(
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
        await using var readCtx = _dogs.CreateContext();
        var reader = new GetDogReader(readCtx);

        var result = await reader.ReadAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        result.Should().BeNull();
    }
}
