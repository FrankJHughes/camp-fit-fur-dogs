using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Domain.Dogs;
using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Dogs.EditDogProfile;

public class EditDogProfileHandler : ICommandHandler<EditDogProfileCommand>
{
    private readonly IDogRepository _dogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditDogProfileHandler(IDogRepository dogRepository, IUnitOfWork unitOfWork)
    {
        _dogRepository = dogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EditDogProfileCommand command, CancellationToken ct)
    {
        var dog = await _dogRepository.GetByIdAsync(
            DogId.From(command.DogId), ct);

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
