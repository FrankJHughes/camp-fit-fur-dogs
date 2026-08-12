using Frank.Core.Application.Abstractions.Cqrs.Queries;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

/// <summary>
/// Represents a query for retrieving a single dog owned by a specific user.
/// <para>
/// This query is part of the Dogs vertical slice and is handled by the
/// <c>GetDogQueryHandler</c> in the application layer. It performs ownership
/// validation and returns a <see cref="GetDogResponse"/> when the dog exists,
/// or <c>null</c> when it does not.
/// </para>
/// <para>
/// The query carries only the identifiers required to locate and authorize
/// access to the dog aggregate. All business rules and domain invariants are
/// enforced by the handler and domain model.
/// </para>
/// </summary>
/// <param name="DogId">
/// The unique identifier of the dog being retrieved.
/// </param>
/// <param name="OwnerId">
/// The unique identifier of the owner requesting the dog. Used to enforce
/// ownership and authorization rules.
/// </param>
public record GetDogQuery(Guid DogId, Guid OwnerId) : IQuery<GetDogResponse?>;
