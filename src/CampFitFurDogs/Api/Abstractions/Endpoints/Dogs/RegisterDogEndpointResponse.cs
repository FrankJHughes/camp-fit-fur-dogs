namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Represents the response returned after successfully registering a new dog
/// in the Camp Fit Fur Dogs system.
/// <para>
/// The response contains the unique identifier assigned to the newly created
/// dog profile.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique identifier of the newly registered dog.
/// </param>
public sealed record RegisterDogEndpointResponse(Guid Id);
