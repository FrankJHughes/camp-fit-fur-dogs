namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

/// <summary>
/// Defines the read‑side abstraction for retrieving a single dog owned by a specific user.
/// <para>
/// This interface belongs to the Dogs vertical slice and is implemented in the
/// infrastructure layer. It is invoked by the <c>GetDogQueryHandler</c> to load
/// presentation‑ready dog data after authorization and domain validation have been
/// performed.
/// </para>
/// <para>
/// The reader returns a <see cref="GetDogResponse"/> when the dog exists and is owned
/// by the requesting user, or <c>null</c> when no matching dog is found.
/// </para>
/// </summary>
public interface IGetDogReader
{
    /// <summary>
    /// Retrieves a dog by its identifier, scoped to the specified owner.
    /// <para>
    /// Implementations are responsible for performing the lookup and projecting
    /// the domain entity into a <see cref="GetDogResponse"/> DTO suitable for API
    /// consumption.
    /// </para>
    /// </summary>
    /// <param name="dogId">
    /// The unique identifier of the dog being retrieved.
    /// </param>
    /// <param name="ownerId">
    /// The unique identifier of the owner requesting the dog. Used to enforce
    /// ownership and authorization rules.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="GetDogResponse"/> when the dog exists and belongs to the owner,
    /// or <c>null</c> if no matching dog is found.
    /// </returns>
    Task<GetDogResponse?> ReadAsync(
        Guid dogId,
        Guid ownerId,
        CancellationToken ct);
}
