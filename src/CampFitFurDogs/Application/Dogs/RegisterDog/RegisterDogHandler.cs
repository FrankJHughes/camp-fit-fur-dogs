
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dog.RegisterDog;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application.Abstractions.Command;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

public sealed class RegisterDogHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IDogRepository _dogRepository;
    private readonly IAppUnitOfWork _unitOfWork;

    public RegisterDogHandler(IDogRepository dogRepository, IAppUnitOfWork unitOfWork)
    {
        _dogRepository = dogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(RegisterDogCommand command, CancellationToken ct)
    {
        if (!Enum.TryParse<Sex>(command.Sex, ignoreCase: true, out var sex))
            throw new ArgumentException("Sex must be 'Male' or 'Female'.");

        var ownerId = UserId.From(command.OwnerId);
        var name = DogName.Create(command.Name);
        var breed = Breed.Create(command.Breed);
        var dob = command.DateOfBirth;

        var dog = Dog.Create(ownerId, name, breed, dob, sex);

        await _dogRepository.AddAsync(dog, ct);
        await _unitOfWork.CommitAsync(ct);

        return dog.Id.Value;
    }
}
