using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.EditDog;
using CampFitFurDogs.Application.Abstractions.UnitOfWork;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Dogs.EditDog;

/// <summary>
/// Handles the <see cref="EditDogCommand"/> by applying updates to an existing
/// <see cref="Dog"/> aggregate and committing the changes atomically.
/// <para>
/// This handler belongs to the Dogs vertical slice and orchestrates the edit
/// workflow. It performs structural validation via <c>EditDogCommandValidator</c>
/// (executed earlier in the pipeline), constructs domain value objects, invokes
/// the write‑side persistence abstraction, and commits the unit of work.
/// </para>
/// <para>
/// All domain invariants are enforced by the <see cref="Dog"/> aggregate and
/// supporting value objects such as <see cref="DogName"/> and <see cref="Breed"/>.
/// </para>
/// </summary>
public class EditDogCommandHandler : ICommandHandler<EditDogCommand>
{
    private readonly IEditDogWriter _writer;
    private readonly IAppUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditDogCommandHandler"/> class.
    /// <para>
    /// The handler requires a write‑side persistence abstraction and an
    /// application‑level unit of work to ensure atomic updates.
    /// </para>
    /// </summary>
    /// <param name="writer">
    /// The write‑side persistence contract responsible for applying dog updates.
    /// </param>
    /// <param name="unitOfWork">
    /// The application‑level unit of work used to commit the edit operation.
    /// </param>
    public EditDogCommandHandler(IEditDogWriter writer, IAppUnitOfWork unitOfWork)
    {
        _writer = writer;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the edit operation by constructing domain value objects,
    /// delegating persistence to <see cref="IEditDogWriter"/>, and committing
    /// the transaction through <see cref="IAppUnitOfWork"/>.
    /// </summary>
    /// <param name="command">
    /// The <see cref="EditDogCommand"/> containing updated dog information.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the operation.
    /// </param>
    public async Task HandleAsync(EditDogCommand command, CancellationToken ct)
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
