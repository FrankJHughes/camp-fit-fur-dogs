using FluentAssertions;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Dogs.GetDogProfile;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;



namespace CampFitFurDogs.Application.Tests.Dogs.GetDogProfile;

public class GetDogProfileHandlerTests
{
    private readonly FakeDogRepository _repo = new();
    private readonly GetDogProfileHandler _handler;

    public GetDogProfileHandlerTests()
    {
        _handler = new GetDogProfileHandler(_repo);
    }

    [Fact]
    public async Task Handle_DogExistsAndOwnedByCustomer_ReturnsProfile()
    {
        var ownerId = CustomerId.From(Guid.NewGuid());
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        await _repo.AddAsync(dog);

        var query = new GetDogProfileQuery(dog.Id.Value, ownerId.Value);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(dog.Id.Value, result!.Id);
        Assert.Equal(ownerId.Value, result.OwnerId);
        Assert.Equal("Biscuit", result.Name);
        Assert.Equal("Golden Retriever", result.Breed);
        Assert.Equal(new DateOnly(2022, 6, 15), result.DateOfBirth);
        Assert.Equal("Female", result.Sex);
    }

    [Fact]
    public async Task Handle_DogNotFound_ResultShouldBeNull()
    {
        var query = new GetDogProfileQuery(Guid.NewGuid(), Guid.NewGuid());
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_DogExistsButNotOwnedByCustomer_ReturnsNull()
    {
        // Arrange
        var ownerA = Guid.NewGuid();
        var ownerB = Guid.NewGuid();

        var dog = Dog.Create(
            CustomerId.From(ownerA),
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female
        );

        var repo = new FakeDogRepository();
        await repo.AddAsync(dog, CancellationToken.None);

        var handler = new GetDogProfileHandler(repo);

        var query = new GetDogProfileQuery(dog.Id.Value, ownerB);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

}
