using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Dogs.EditDogProfile;

public class EditDogProfileHandler : ICommandHandler<EditDogProfileCommand>
{
    private readonly IGetDogByIdReader _reader;
    private readonly IAppUnitOfWork _unitOfWork;

    public EditDogProfileHandler(IGetDogByIdReader reader, IAppUnitOfWork unitOfWork)
    {
        _reader = reader;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(EditDogProfileCommand command, CancellationToken ct)
    {
        var dog = await _reader.ReadAsync(command.DogId, ct);

        if (dog is null || !dog.OwnerId.Value.Equals(command.OwnerId))
            throw new InvalidOperationException("Dog not found.");

        dog.Update(
            DogName.Create(command.Name),
            Breed.Create(command.Breed),
            command.DateOfBirth,
            Enum.Parse<Sex>(command.Sex, ignoreCase: true));

        await _unitOfWork.CommitAsync(ct);
    }
}
