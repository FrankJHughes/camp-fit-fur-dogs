using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;

namespace CampFitFurDogs.Infrastructure.Tests.Dogs;

public class GetDogProfileReaderTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public GetDogProfileReaderTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<(CustomerId OwnerId, Dog Dog)> SeedDogAsync()
    {
        await using var ctx = _fixture.CreateContext();

        var customer = new CustomerBuilder()
            .WithFirstName(CustomerFixtures.First.Value)
            .WithLastName(CustomerFixtures.Last.Value)
            .WithEmail($"reader-{Guid.NewGuid()}@example.com")
            .WithPhone(CustomerFixtures.Phone.Value)
            .WithPassword(PasswordFixtures.Plain)
            .Build();

        await new CustomerRepository(ctx).AddAsync(customer, CancellationToken.None);

        var dog = new DogBuilder()
            .WithOwner(customer.Id)
            .WithName("Biscuit")
            .WithBreed("Golden Retriever")
            .BornOn(new DateOnly(2022, 6, 15))
            .WithSex(Sex.Female)
            .Build();

        await new DogRepository(ctx).AddAsync(dog, CancellationToken.None);
        await ctx.SaveChangesAsync();

        return (customer.Id, dog);
    }

    [Fact]
    public async Task GetDogProfileAsync_DogExistsAndOwnedByCustomer_ReturnsProfile()
    {
        var (ownerId, dog) = await SeedDogAsync();

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
        result.Sex.Should().Be("Female");
    }

    [Fact]
    public async Task GetDogProfileAsync_DogNotFound_ReturnsNull()
    {
        await using var ctx = _fixture.CreateContext();
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

        await using var readCtx = _fixture.CreateContext();
        var reader = new GetDogProfileReader(readCtx);

        var result = await reader.GetDogProfileAsync(
            dog.Id.Value,
            wrongOwnerId,
            CancellationToken.None);

        result.Should().BeNull();
    }
}
