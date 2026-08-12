namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

/// <summary>
/// Represents the data returned when retrieving a single dog owned by a user.
/// <para>
/// This response DTO is part of the Dogs vertical slice and is used by the
/// <c>GetDogEndpoint</c> to expose dog information to API clients.
/// </para>
/// <para>
/// The record contains only presentation‑safe fields. Domain invariants and
/// authorization checks are enforced earlier in the pipeline.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique identifier of the dog.
/// </param>
/// <param name="OwnerId">
/// The unique identifier of the owner who registered the dog.
/// </param>
/// <param name="Name">
/// The dog’s name.
/// </param>
/// <param name="Breed">
/// The dog’s breed.
/// </param>
/// <param name="DateOfBirth">
/// The dog’s date of birth.
/// </param>
/// <param name="Sex">
/// The dog’s sex (e.g., "Male", "Female").
/// </param>
public record GetDogResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
