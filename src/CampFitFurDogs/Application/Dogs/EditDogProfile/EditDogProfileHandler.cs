using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDogProfile;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Dogs.EditDogProfile;

public class EditDogProfileHandler : ICommandHandler<EditDogProfileCommand>
{
    private readonly IEditDogProfileWriter _writer;
    private readonly IAppUnitOfWork _unitOfWork;

    public EditDogProfileHandler(IEditDogProfileWriter writer, IAppUnitOfWork unitOfWork)
    {
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(EditDogProfileCommand command, CancellationToken ct)
    {
        await _writer.WriteAsync(
            UserId.From(command.OwnerId),
            DogId.From(command.DogId),
            DogName.Create(command.Name),
            Breed.Create(command.Breed),
            command.DateOfBirth,
            Enum.Parse<Sex>(command.Sex, ignoreCase: true),
            ct);
        await _unitOfWork.CommitAsync(ct);
    }
}
