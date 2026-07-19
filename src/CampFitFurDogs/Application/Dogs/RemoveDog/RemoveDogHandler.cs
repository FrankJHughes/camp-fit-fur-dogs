using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Dogs.RemoveDog;

public sealed class RemoveDogHandler : ICommandHandler<RemoveDogCommand>
{
    private readonly IGetDogByIdReader _dogReader;
    private readonly IRemoveDogWriter _dogWriter;
    private readonly IAppUnitOfWork _unitOfWork;

    public RemoveDogHandler(
        IGetDogByIdReader dogReader,
        IRemoveDogWriter dogWriter,
        IAppUnitOfWork unitOfWork)
    {
        _dogReader = dogReader;
        _dogWriter = dogWriter;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(RemoveDogCommand command, CancellationToken cancellationToken)
    {
        var dogId = command.DogId;
        var ownerId = command.OwnerId;

        var response = await _dogReader.ReadAsync(dogId, cancellationToken);
        if (response is null || response.OwnerId.Value != ownerId)
        {
            throw new InvalidOperationException($"Dog {command.DogId} not found.");
        }

        await _dogWriter.WriteAsync(dogId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
