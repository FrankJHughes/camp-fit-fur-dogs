using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Domain.Dogs;
using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Dogs.RemoveDog;

public sealed class RemoveDogHandler : ICommandHandler<RemoveDogCommand>
{
    private readonly IDogRepository _dogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveDogHandler(IDogRepository dogRepository, IUnitOfWork unitOfWork)
    {
        _dogRepository = dogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(RemoveDogCommand command, CancellationToken cancellationToken)
    {
        var dogId = DogId.From(command.DogId);
        var dog = await _dogRepository.GetByIdAsync(dogId, cancellationToken)
            ?? throw new InvalidOperationException($"Dog {command.DogId} not found.");

        if (dog.OwnerId.Value != command.OwnerId)
            throw new InvalidOperationException($"Dog {command.DogId} does not belong to owner {command.OwnerId}.");

        await _dogRepository.DeleteAsync(dog, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
