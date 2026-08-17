using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

/// <summary>
/// Defines the write‑side persistence contract for registering a new dog.
/// <para>
/// This abstraction belongs to the Dogs vertical slice and is implemented in the
/// infrastructure layer. It is invoked by the <c>RegisterDogCommandHandler</c>
/// after all validation, authorization, and domain rules have been applied.
/// </para>
/// <para>
/// The writer is responsible for persisting a fully‑constructed <see cref="Dog"/>
/// aggregate into the underlying storage mechanism.
/// </para>
/// </summary>
public interface IRegisterDogWriter
{
    /// <summary>
    /// Persists a newly created dog aggregate.
    /// <para>
    /// Implementations are responsible for inserting the dog into the persistence
    /// store and ensuring that all domain invariants have already been satisfied
    /// by the application layer and domain model.
    /// </para>
    /// </summary>
    /// <param name="dog">
    /// The fully‑constructed <see cref="Dog"/> aggregate to be persisted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to observe cancellation of the write operation.
    /// </param>
    Task WriteAsync(Dog dog, CancellationToken cancellationToken = default);
}
