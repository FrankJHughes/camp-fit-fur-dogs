
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

public sealed class RegisterDogHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IRegisterDogWriter _dogWriter;
    private readonly IAppUnitOfWork _unitOfWork;

    public RegisterDogHandler(IRegisterDogWriter dogRepository, IAppUnitOfWork unitOfWork)
    {
        _dogWriter = dogRepository;
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

        await _dogWriter.WriteAsync(dog, ct);
        await _unitOfWork.CommitAsync(ct);

        return dog.Id.Value;
    }
}
