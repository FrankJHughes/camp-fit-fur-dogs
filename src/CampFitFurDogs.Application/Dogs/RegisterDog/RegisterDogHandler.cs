using SharedKernel.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

public sealed class RegisterDogHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IDogRepository _dogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterDogHandler(IDogRepository dogRepository, IUnitOfWork unitOfWork)
    {
        _dogRepository = dogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegisterDogCommand command, CancellationToken ct)
    {
        if (!Enum.TryParse<Sex>(command.Sex, ignoreCase: true, out var sex))
            throw new ArgumentException("Sex must be 'Male' or 'Female'.");

        var ownerId = CustomerId.From(command.OwnerId);
        var name = DogName.Create(command.Name);
        var breed = Breed.Create(command.Breed);
        var dob = command.DateOfBirth;

        var dog = Dog.Create(ownerId, name, breed, dob, sex);

        await _dogRepository.AddAsync(dog, ct);
        await _unitOfWork.CommitAsync(ct);

        return dog.Id.Value;
    }
}
