using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;

/// <summary>
/// Defines the read‑side abstraction for retrieving a dog by its unique identifier,
/// without performing ownership validation.
/// <para>
/// This interface belongs to the Dogs vertical slice and is implemented in the
/// infrastructure layer. It is typically used in scenarios where the caller already
/// has permission to access the dog, or where ownership is validated elsewhere in
/// the pipeline.
/// </para>
/// <para>
/// The reader returns a <see cref="Dog"/> domain entity when found, or <c>null</c>
/// when no matching dog exists.
/// </para>
/// </summary>
public interface IGetDogByIdReader
{
    /// <summary>
    /// Retrieves a dog by its identifier.
    /// <para>
    /// Implementations are responsible for locating the dog aggregate and returning
    /// the domain entity directly, without projecting it into a DTO. This is useful
    /// for internal application workflows that require full domain behavior.
    /// </para>
    /// </summary>
    /// <param name="dogId">
    /// The unique identifier of the dog being retrieved.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the read operation.
    /// </param>
    /// <returns>
    /// The <see cref="Dog"/> domain entity when found, or <c>null</c> if no matching
    /// dog exists.
    /// </returns>
    Task<Dog?> ReadAsync(Guid dogId, CancellationToken ct);
}
