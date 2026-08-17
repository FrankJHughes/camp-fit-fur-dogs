namespace CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;

/// <summary>
/// Defines the write‑side persistence contract for removing an existing dog.
/// <para>
/// This abstraction belongs to the Dogs vertical slice and is implemented in the
/// infrastructure layer. It is invoked by the <c>RemoveDogCommandHandler</c>
/// after all validation, authorization, and domain rules have been applied.
/// </para>
/// <para>
/// The writer is responsible for deleting the dog aggregate from the underlying
/// persistence mechanism.
/// </para>
/// </summary>
public interface IRemoveDogWriter
{
    /// <summary>
    /// Removes the specified dog from the persistence store.
    /// <para>
    /// Implementations are responsible for locating the dog aggregate and
    /// performing a permanent delete operation. All domain invariants and
    /// authorization checks are enforced earlier in the pipeline.
    /// </para>
    /// </summary>
    /// <param name="dogId">
    /// The unique identifier of the dog to be removed.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to observe cancellation of the delete operation.
    /// </param>
    Task WriteAsync(Guid dogId, CancellationToken cancellationToken = default);
}
