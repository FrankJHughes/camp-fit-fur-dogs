
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.Infrastructure.Persistence;
using CampFitFurDogs.TestUtilities.Builders;
using Frank.Identity.Domain.Users;
using Frank.Identity.EntityFrameworkCore.Persistence;
using Frank.Identity.EntityFrameworkCore.Users;
using Frank.TestUtilities.Builders;
using Frank.TestUtilities.Fixtures;

namespace CampFitFurDogs.Infrastructure.Tests.Dogs;

public class GetDogProfileReaderTests :
    IClassFixture<PostgresFixture<FrankIdentityDbContext>>,
    IClassFixture<PostgresFixture<AppDbContext>>
{
    private readonly PostgresFixture<FrankIdentityDbContext> _identity;
    private readonly PostgresFixture<AppDbContext> _dogs;

    public GetDogProfileReaderTests(
        PostgresFixture<FrankIdentityDbContext> identity,
        PostgresFixture<AppDbContext> dogs)
    {
        _identity = identity;
        _dogs = dogs;
    }

    private async Task<(UserId OwnerId, Domain.Dogs.Dog Dog)> SeedDogAsync()
    {
        // Seed User (Owner) in Identity DB
        await using var identityCtx = _identity.CreateContext();
        var user = new UserBuilder()
            .WithFirstName(UserFixtures.First.Value)
            .WithLastName(UserFixtures.Last.Value)
            .WithEmail($"reader-{Guid.NewGuid()}@example.com")
            .WithPhone(UserFixtures.Phone.Value)
            .Build();

        await new UserRepository(identityCtx).AddAsync(user, CancellationToken.None);
        await identityCtx.SaveChangesAsync();

        // Seed Dog in Dogs DB
        await using var dogsCtx = _dogs.CreateContext();
        var dog = new DogBuilder()
            .WithOwner(user.Id)
            .WithName("Biscuit")
            .WithBreed("Golden Retriever")
            .BornOn(new DateOnly(2022, 6, 15))
            .WithSex(Sex.Female)
            .Build();

        await new DogRepository(dogsCtx).AddAsync(dog, CancellationToken.None);
        await dogsCtx.SaveChangesAsync();

        return (user.Id, dog);
    }

    [Fact]
    public async Task GetDogProfileAsync_DogExistsAndOwnedByUser_ReturnsProfile()
    {
        var (ownerId, dog) = await SeedDogAsync();

        await using var readCtx = _dogs.CreateContext();
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
        result.Sex.Should().Be("Female");
    }

    [Fact]
    public async Task GetDogProfileAsync_DogNotFound_ReturnsNull()
    {
        await using var ctx = _dogs.CreateContext();
        var reader = new GetDogProfileReader(ctx);

        var result = await reader.GetDogProfileAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDogProfileAsync_DogExistsButWrongOwner_ReturnsNull()
    {
        var (_, dog) = await SeedDogAsync();
        var wrongOwnerId = Guid.NewGuid();

        await using var readCtx = _dogs.CreateContext();
        var reader = new GetDogProfileReader(readCtx);

        var result = await reader.GetDogProfileAsync(
            dog.Id.Value,
            wrongOwnerId,
            CancellationToken.None);

        result.Should().BeNull();
    }
}
