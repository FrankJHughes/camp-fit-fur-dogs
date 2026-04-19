using FluentAssertions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Dogs;

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

        var customer = Customer.Create(
            "Frank",
            "Hughes",
            Email.From($"reader-{Guid.NewGuid()}@example.com"),
            PhoneNumber.From("555-9876"),
            PasswordHash.From(
                Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes("TestPass123!"))));

        await new CustomerRepository(ctx).AddAsync(customer, CancellationToken.None);

        var dog = Dog.Create(
            customer.Id,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

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
            dog.Id.Value, ownerId.Value, CancellationToken.None);

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
            Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

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
            dog.Id.Value, wrongOwnerId, CancellationToken.None);

        result.Should().BeNull();
    }
}
