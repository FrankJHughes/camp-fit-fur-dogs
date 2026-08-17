using CampFitFurDogs.Domain.Dogs;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Abstractions.Dogs;

/// <summary>
/// Defines the write‑side persistence contract for updating an existing dog.
/// <para>
/// This abstraction belongs to the Dogs vertical slice and is implemented in the
/// infrastructure layer. It is invoked by the <c>EditDogCommandHandler</c> to
/// persist changes to a dog aggregate after all validation and business rules
/// have been applied.
/// </para>
/// <para>
/// The interface exposes a single operation that updates the dog’s core attributes
/// while enforcing ownership and domain invariants.
/// </para>
/// </summary>
public interface IEditDogWriter
{
    /// <summary>
    /// Persists updates to an existing dog owned by the specified user.
    /// <para>
    /// Implementations are responsible for locating the dog aggregate,
    /// applying the updated values, and committing the changes to the underlying
    /// persistence mechanism.
    /// </para>
    /// </summary>
    /// <param name="ownerId">
    /// The identifier of the owner performing the update. Used to enforce
    /// ownership and authorization rules.
    /// </param>
    /// <param name="id">
    /// The unique identifier of the dog being updated.
    /// </param>
    /// <param name="name">
    /// The updated <see cref="DogName"/> value.
    /// </param>
    /// <param name="breed">
    /// The updated <see cref="Breed"/> value.
    /// </param>
    /// <param name="dateOfBirth">
    /// The updated date of birth for the dog.
    /// </param>
    /// <param name="sex">
    /// The updated <see cref="Sex"/> value.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to observe cancellation of the operation.
    /// </param>
    Task WriteAsync(
        UserId ownerId,
        DogId id,
        DogName name,
        Breed breed,
        DateOnly dateOfBirth,
        Sex sex,
        CancellationToken cancellationToken);
}
