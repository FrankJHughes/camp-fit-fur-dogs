namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

/// <summary>
/// Represents a lightweight summary of a dog owned by a specific user.
/// <para>
/// This DTO is part of the Dogs vertical slice and is used when returning
/// collections of dogs, such as in the <c>ListDogsByOwnerResponse</c>.
/// </para>
/// <para>
/// The record contains only presentation‑safe fields and does not expose
/// domain entities directly.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique identifier of the dog.
/// </param>
/// <param name="Name">
/// The dog’s name.
/// </param>
/// <param name="Breed">
/// The dog’s breed.
/// </param>
public record DogSummary(Guid Id, string Name, string Breed);
