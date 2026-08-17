namespace CampFitFurDogs.Api.Endpoints.Dogs;

/// <summary>
/// Represents the response returned by the <c>GetDogEndpoint</c>, containing
/// all core profile information for a registered dog.
/// <para>
/// This model is used by staff and owners to view a dog's identity, ownership,
/// and biological details.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique identifier of the dog within the Camp Fit Fur Dogs system.
/// </param>
/// <param name="OwnerId">
/// The unique identifier of the dog's owner. This links the dog to its customer record.
/// </param>
/// <param name="Name">
/// The dog's display name as provided during registration or subsequent edits.
/// </param>
/// <param name="Breed">
/// The dog's breed, used for classification, care requirements, and reporting.
/// </param>
/// <param name="DateOfBirth">
/// The dog's date of birth represented as a <see cref="DateOnly"/> value.
/// </param>
/// <param name="Sex">
/// The dog's biological sex (e.g., <c>Male</c>, <c>Female</c>).
/// </param>
public record GetDogEndpointResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
