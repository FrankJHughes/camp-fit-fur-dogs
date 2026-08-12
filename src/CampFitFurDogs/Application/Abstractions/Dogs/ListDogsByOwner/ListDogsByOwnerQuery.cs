using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

/// <summary>
/// Represents a query for retrieving all dogs owned by a specific user.
/// <para>
/// This query is part of the Dogs vertical slice and is handled by the
/// <c>ListDogsByOwnerQueryHandler</c> in the application layer. It performs
/// ownership‑scoped retrieval and returns a <see cref="ListDogsByOwnerResponse"/>
/// containing all dogs registered to the specified owner.
/// </para>
/// <para>
/// The query carries only the owner identifier required to locate and authorize
/// access to the dog collection. All business rules and domain invariants are
/// enforced by the handler and domain model.
/// </para>
/// </summary>
/// <param name="OwnerId">
/// The unique identifier of the owner whose dogs should be retrieved.
/// </param>
public record ListDogsByOwnerQuery(Guid OwnerId) : IQuery<ListDogsByOwnerResponse>;
