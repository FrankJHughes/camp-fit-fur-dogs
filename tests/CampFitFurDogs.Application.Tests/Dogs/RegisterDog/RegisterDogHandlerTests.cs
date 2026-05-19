using FluentAssertions;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;

using CampFitFurDogs.TestUtilities.Fixtures;

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
            Name: DogFixtures.DefaultName,
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString());

        var dogId = await _handler.HandleAsync(command, CancellationToken.None);

        dogId.Should().NotBe(Guid.Empty);
        _repo.Dogs.Should().HaveCount(1);

        var dog = _repo.Dogs[0];
        dog.Name.Value.Should().Be(DogFixtures.DefaultName);
        dog.Breed.Value.Should().Be(DogFixtures.DefaultBreed);
        dog.DateOfBirth.Should().Be(DogFixtures.Dob);
        dog.Sex.Should().Be(DogFixtures.Sex);
    }

    [Fact]
    public async Task Handle_InvalidSex_ThrowsArgumentException()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: DogFixtures.DefaultName,
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: "Unknown");

        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command, CancellationToken.None));

        _repo.Dogs.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_CommitsUnitOfWork()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: DogFixtures.DefaultName,
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: DogFixtures.Sex.ToString());

        await _handler.HandleAsync(command, CancellationToken.None);

        _unitOfWork.Committed.Should().BeTrue();
        _unitOfWork.CommitCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_InvalidSex_ThrowsAndDoesNotCommit()
    {
        var command = new RegisterDogCommand(
            OwnerId: Guid.NewGuid(),
            Name: DogFixtures.DefaultName,
            Breed: DogFixtures.DefaultBreed,
            DateOfBirth: DogFixtures.Dob,
            Sex: "Unknown");

        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command, CancellationToken.None));

        _unitOfWork.Committed.Should().BeFalse();
        _unitOfWork.CommitCount.Should().Be(0);
    }
}
