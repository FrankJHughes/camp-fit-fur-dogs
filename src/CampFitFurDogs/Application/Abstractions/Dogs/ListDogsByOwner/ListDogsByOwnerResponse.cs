namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;


/// <summary>
/// Represents the response returned when listing all dogs owned by a specific user.
/// <para>
/// This DTO is produced by the <c>ListDogsByOwnerQueryHandler</c> and consumed
/// by the corresponding API endpoint. It contains a collection of
/// <see cref="DogSummary"/> items, each representing a single dog.
/// </para>
/// <para>
/// The response is immutable and presentation‑safe, containing only the data
/// required by the client.
/// </para>
/// </summary>
/// <param name="Dogs">
/// A read‑only collection of <see cref="DogSummary"/> records representing all
/// dogs owned by the specified user.
/// </param>
public record ListDogsByOwnerResponse(IReadOnlyList<DogSummary> Dogs);
