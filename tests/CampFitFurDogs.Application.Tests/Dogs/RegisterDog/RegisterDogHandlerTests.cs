using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Dogs.RegisterDog;

public class RegisterDogHandlerTests
{
    private readonly FakeDogRepository _repo = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly RegisterDogHandler _handler;

    public RegisterDogHandlerTests()
    {
        _handler = new RegisterDogHandler(_repo, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsDogAndReturnsId()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: "Biscuit",
            Breed: "Golden Retriever",
            DateOfBirth: new DateOnly(2022, 6, 15),
            Sex: "Female");

        var dogId = await _handler.HandleAsync(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, dogId);
        Assert.Single(_repo.Dogs);

        var dog = _repo.Dogs[0];
        Assert.Equal("Biscuit", dog.Name.Value);
        Assert.Equal("Golden Retriever", dog.Breed.Value);
        Assert.Equal(new DateOnly(2022, 6, 15), dog.DateOfBirth);
        Assert.Equal(Sex.Female, dog.Sex);
    }

    [Fact]
    public async Task Handle_InvalidSex_ThrowsArgumentException()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: "Biscuit",
            Breed: "Poodle",
            DateOfBirth: new DateOnly(2023, 1, 1),
            Sex: "Unknown");

        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command, CancellationToken.None));

        Assert.Empty(_repo.Dogs);
    }

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: "Biscuit",
            Breed: "Golden Retriever",
            DateOfBirth: new DateOnly(2022, 6, 15),
            Sex: "Female");

        await _handler.HandleAsync(command, CancellationToken.None);

        Assert.True(_unitOfWork.Committed);
        Assert.Equal(1, _unitOfWork.CommitCount);
    }

    [Fact]
    public async Task Handle_InvalidSex_ThrowsAndDoesNotCommit()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: "Biscuit",
            Breed: "Poodle",
            DateOfBirth: new DateOnly(2023, 1, 1),
            Sex: "Unknown");

        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command, CancellationToken.None));

        Assert.False(_unitOfWork.Committed);
        Assert.Equal(0, _unitOfWork.CommitCount);
    }

}
