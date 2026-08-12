namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Represents a lightweight summary of a dog profile, typically used in list
/// views or search results where full dog details are not required.
/// </summary>
/// <param name="Id">
/// The unique identifier of the dog within the Camp Fit Fur Dogs system.
/// </param>
/// <param name="Name">
/// The dog's display name.
/// </param>
/// <param name="Breed">
/// The dog's breed, used for classification and quick identification.
/// </param>
public record GetDogSummaryEndpointResponse(Guid Id, string Name, string Breed);
