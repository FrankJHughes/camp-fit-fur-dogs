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

        Assert.Equal(dog.Id.Value, result.Id);
        Assert.Equal(ownerId.Value, result.OwnerId);
        Assert.Equal("Biscuit", result.Name);
        Assert.Equal("Golden Retriever", result.Breed);
        Assert.Equal(new DateOnly(2022, 6, 15), result.DateOfBirth);
        Assert.Equal("Female", result.Sex);
    }

    [Fact]
    public async Task Handle_DogNotFound_ThrowsKeyNotFoundException()
    {
        var query = new GetDogProfileQuery(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DogExistsButNotOwnedByCustomer_ThrowsUnauthorizedAccessException()
    {
        var ownerId = CustomerId.From(Guid.NewGuid());
        var dog = Dog.Create(
            ownerId,
            DogName.Create("Biscuit"),
            Breed.Create("Golden Retriever"),
            new DateOnly(2022, 6, 15),
            Sex.Female);

        await _repo.AddAsync(dog);

        var differentCustomerId = Guid.NewGuid();
        var query = new GetDogProfileQuery(dog.Id.Value, differentCustomerId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

}
