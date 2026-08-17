using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Dogs.ListDogsByOwner;

/// <summary>
/// Handles the <see cref="ListDogsByOwnerQuery"/> by retrieving all dogs
/// belonging to a specific owner and projecting them into a
/// <see cref="ListDogsByOwnerResponse"/> DTO.
/// <para>
/// This handler belongs to the Dogs vertical slice and orchestrates the
/// read‑side workflow. It delegates retrieval to the
/// <see cref="IListDogsByOwnerReader"/> implementation and performs no domain
/// logic itself.
/// </para>
/// <para>
/// Ownership validation and projection are handled by the reader in the
/// infrastructure layer.
/// </para>
public sealed class ListDogsByOwnerQueryHandler(IListDogsByOwnerReader reader)
    : IQueryHandler<ListDogsByOwnerQuery, ListDogsByOwnerResponse>
{
    /// <summary>
    /// Executes the dog‑listing workflow by delegating to the
    /// <see cref="IListDogsByOwnerReader"/> implementation.
    /// </summary>
    /// <param name="query">
    /// The <see cref="ListDogsByOwnerQuery"/> containing the owner's identifier.
    /// </param>
    /// <param name="ct">
    /// A cancellation token used to observe cancellation of the read operation.
    /// </param>
    /// <returns>
    /// A <see cref="ListDogsByOwnerResponse"/> containing all dogs owned by the
    /// specified user.
    /// </returns>
    public async Task<ListDogsByOwnerResponse> HandleAsync(
        ListDogsByOwnerQuery query, CancellationToken ct)
        => await reader.ReadAsync(query.OwnerId, ct);
}
