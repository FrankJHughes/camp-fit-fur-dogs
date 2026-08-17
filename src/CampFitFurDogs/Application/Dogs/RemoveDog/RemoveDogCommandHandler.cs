using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Application.Abstractions.UnitOfWork;
using Frank.Core.Application.Abstractions.Cqrs.Commands;

namespace CampFitFurDogs.Application.Dogs.RemoveDog;

/// <summary>
/// Handles the <see cref="RemoveDogCommand"/> by validating ownership,
/// removing the specified dog, and committing the operation atomically.
/// <para>
/// This handler belongs to the Dogs vertical slice and orchestrates the
/// removal workflow. It performs structural validation earlier in the pipeline
/// via <c>RemoveDogCommandValidator</c>, retrieves the dog using the read‑side
/// abstraction, verifies ownership, delegates deletion to the write‑side
/// abstraction, and commits the transaction.
/// </para>
/// <para>
/// All domain invariants are enforced by the <c>Dog</c> aggregate and its
/// supporting value objects. This handler ensures that only the rightful owner
/// can remove a dog.
/// </para>
/// </summary>
public sealed class RemoveDogCommandHandler : ICommandHandler<RemoveDogCommand>
{
    private readonly IGetDogByIdReader _dogReader;
    private readonly IRemoveDogWriter _dogWriter;
    private readonly IAppUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveDogCommandHandler"/> class.
    /// <para>
    /// The handler requires a read‑side abstraction to verify the dog exists and
    /// belongs to the caller, a write‑side abstraction to perform the removal,
    /// and an application‑level unit of work to commit the operation atomically.
    /// </para>
    /// </summary>
    /// <param name="dogReader">
    /// The read‑side abstraction used to retrieve the dog by its identifier.
    /// </param>
    /// <param name="dogWriter">
    /// The write‑side abstraction responsible for removing the dog.
    /// </param>
    /// <param name="unitOfWork">
    /// The application‑level unit of work used to commit the removal operation.
    /// </param>
    public RemoveDogCommandHandler(
        IGetDogByIdReader dogReader,
        IRemoveDogWriter dogWriter,
        IAppUnitOfWork unitOfWork)
    {
        _dogReader = dogReader;
        _dogWriter = dogWriter;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the dog removal workflow by verifying ownership, delegating
    /// deletion to <see cref="IRemoveDogWriter"/>, and committing the transaction
    /// through <see cref="IAppUnitOfWork"/>.
    /// </summary>
    /// <param name="command">
    /// The <see cref="RemoveDogCommand"/> containing the dog and owner identifiers.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token used to observe cancellation of the operation.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the dog does not exist or does not belong to the requesting owner.
    /// </exception>
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
