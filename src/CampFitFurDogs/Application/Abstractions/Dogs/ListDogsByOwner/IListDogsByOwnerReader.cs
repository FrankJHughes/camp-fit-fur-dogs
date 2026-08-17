namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

/// <summary>
/// Defines the read‑side abstraction for retrieving all dogs owned by a specific user.
/// <para>
/// This interface belongs to the Dogs vertical slice and is implemented in the
/// infrastructure layer. It is invoked by the <c>ListDogsByOwnerQueryHandler</c>
/// to load presentation‑ready dog data for API consumption.
/// </para>
/// <para>
/// The reader returns a <see cref="ListDogsByOwnerResponse"/> containing the full
/// set of dogs owned by the specified user.
/// </para>
/// </summary>
public interface IListDogsByOwnerReader
{
    /// <summary>
    /// Retrieves all dogs registered to the specified owner.
    /// <para>
    /// Implementations are responsible for performing the lookup and projecting
    /// domain entities into a <see cref="ListDogsByOwnerResponse"/> DTO suitable
    /// for API consumption.
    /// </para>
    /// </summary>
    /// <param name="ownerId">
    /// The unique identifier of the owner whose dogs should be retrieved.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="ListDogsByOwnerResponse"/> containing all dogs owned by the user.
    /// </returns>
    Task<ListDogsByOwnerResponse> ReadAsync(
        Guid ownerId,
        CancellationToken ct);
}
