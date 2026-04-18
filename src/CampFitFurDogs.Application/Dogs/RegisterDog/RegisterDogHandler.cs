using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

public sealed class RegisterDogHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IDogRepository _dogRepository;

    public RegisterDogHandler(IDogRepository dogRepository)
    {
        _dogRepository = dogRepository;
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

        return dog.Id.Value;
    }
}
