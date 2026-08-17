using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Abstractions.UnitOfWork;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Dogs.RegisterDog;

/// <summary>
/// Handles the <see cref="RegisterDogCommand"/> by creating a new <see cref="Dog"/>
/// aggregate and committing it atomically through the application unit of work.
/// <para>
/// This handler belongs to the Dogs vertical slice and orchestrates the
/// registration workflow. It performs structural validation earlier in the
/// pipeline via <c>RegisterDogCommandValidator</c>, constructs domain value
/// objects, invokes the write‑side persistence abstraction, and commits the
/// transaction.
/// </para>
/// <para>
/// All domain invariants are enforced by the <see cref="Dog"/> aggregate and its
/// supporting value objects such as <see cref="DogName"/> and <see cref="Breed"/>.
/// </para>
/// </summary>
public sealed class RegisterDogCommandHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IRegisterDogWriter _dogWriter;
    private readonly IAppUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDogCommandHandler"/> class.
    /// <para>
    /// The handler requires a write‑side persistence abstraction and an
    /// application‑level unit of work to ensure atomic creation of the dog
    /// aggregate.
    /// </para>
    /// </summary>
    /// <param name="dogRepository">
    /// The write‑side persistence contract responsible for storing newly created dogs.
    /// </param>
    /// <param name="unitOfWork">
    /// The application‑level unit of work used to commit the registration operation.
    /// </param>
    public RegisterDogCommandHandler(IRegisterDogWriter dogRepository, IAppUnitOfWork unitOfWork)
    {
        _dogWriter = dogRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the dog registration workflow by constructing domain value objects,
    /// creating the <see cref="Dog"/> aggregate, delegating persistence to
    /// <see cref="IRegisterDogWriter"/>, and committing the transaction through
    /// <see cref="IAppUnitOfWork"/>.
    /// </summary>
    /// <param name="command">
    /// The <see cref="RegisterDogCommand"/> containing the new dog's information.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the operation.
    /// </param>
    /// <returns>
    /// The unique identifier of the newly registered dog.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided sex value cannot be parsed into a valid <see cref="Sex"/> enum.
    /// </exception>
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
