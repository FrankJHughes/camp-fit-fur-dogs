using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Dogs;

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

        var customer = Customer.Create(
            "Frank",
            "Hughes",
            Email.From($"infra-{Guid.NewGuid()}@example.com"),
            PhoneNumber.From("555-1234"),
            PasswordHash.From(
                Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes("SuperSecure123!"))));

        await repo.AddAsync(customer, CancellationToken.None);
        return customer.Id;
    }

    [Fact]
    public async Task AddAsync_PersistsDogWithCorrectMapping()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _fixture.CreateContext();
        var repo = new DogRepository(ctx);

        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        await repo.AddAsync(dog, CancellationToken.None);

        await using var readCtx = _fixture.CreateContext();
        var all = await readCtx.Set<Dog>().AsNoTracking().ToListAsync();
        var persisted = all.Single(d => d.Id.Value == dog.Id.Value);

        persisted.OwnerId.Should().Be(ownerId);
        persisted.Name.Should().Be(DogName.Create("Biscuit"));
        persisted.Breed.Should().Be(Breed.Create("Golden Retriever"));
        persisted.DateOfBirth.Should().Be(new DateOnly(2022, 6, 15));
        persisted.Sex.Should().Be(Sex.Female);
    }

    [Fact]
    public async Task AddAsync_WithNonExistentOwner_ThrowsOnSave()
    {
        await using var ctx = _fixture.CreateContext();
        var repo = new DogRepository(ctx);

        var fakeOwnerId = CustomerId.From(Guid.NewGuid());

        var dog = Dog.Create(
            fakeOwnerId,
            DogName.Create("Ghost"),
            Breed.Create("Husky"),
            new DateOnly(2023, 1, 1),
            Sex.Male);

        var act = () => repo.AddAsync(dog, CancellationToken.None);

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task AddAsync_PersistsMultipleDogsForSameOwner()
    {
        var ownerId = await SeedOwnerAsync();

        await using var ctx = _fixture.CreateContext();
        var repo = new DogRepository(ctx);

        var dog1 = Dog.Create(
            ownerId,
            DogName.Create("Maple"),
            Breed.Create("Poodle"),
            new DateOnly(2023, 3, 10),
            Sex.Female);

        var dog2 = Dog.Create(
            ownerId,
            DogName.Create("Bandit"),
            Breed.Create("Beagle"),
            new DateOnly(2021, 8, 5),
            Sex.Male);

        await repo.AddAsync(dog1, CancellationToken.None);
        await repo.AddAsync(dog2, CancellationToken.None);

        await using var readCtx = _fixture.CreateContext();
        var allDogs = await readCtx.Set<Dog>().AsNoTracking().ToListAsync();
        var ownerDogs = allDogs.Where(d => d.OwnerId == ownerId).ToList();

        ownerDogs.Should().HaveCount(2);
    }
}
