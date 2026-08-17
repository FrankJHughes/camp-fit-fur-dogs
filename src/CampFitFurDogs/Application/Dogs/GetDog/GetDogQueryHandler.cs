using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Dogs.GetDog;

/// <summary>
/// Handles the <see cref="GetDogQuery"/> by retrieving a single dog owned by a
/// specific user and projecting it into a <see cref="GetDogResponse"/> DTO.
/// <para>
/// This handler belongs to the Dogs vertical slice and orchestrates the read
/// workflow. It delegates retrieval to the <see cref="IGetDogReader"/> and
/// performs no domain logic itself.
/// </para>
/// <para>
/// Ownership validation and projection are handled by the reader implementation
/// in the infrastructure layer.
/// </para>
/// </summary>
public sealed class GetDogQueryHandler(IGetDogReader reader)
    : IQueryHandler<GetDogQuery, GetDogResponse?>
{
    /// <summary>
    /// Executes the dog‑retrieval workflow by delegating to the
    /// <see cref="IGetDogReader"/> implementation.
    /// </summary>
    /// <param name="query">
    /// The <see cref="GetDogQuery"/> containing the dog and owner identifiers.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="GetDogResponse"/> if the dog exists and belongs to the owner;
    /// otherwise <c>null</c>.
    /// </returns>
    public async Task<GetDogResponse?> HandleAsync(
        GetDogQuery query, CancellationToken ct)
        => await reader.ReadAsync(query.DogId, query.OwnerId, ct);
}
