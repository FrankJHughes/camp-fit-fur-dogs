namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Represents the payload required to edit an existing dog profile.
/// <para>
/// This request is used by the <c>EditDogEndpoint</c> to update core dog
/// attributes such as name, breed, date of birth, and sex.
/// </para>
/// </summary>
/// <param name="Name">
/// The dog's display name. This is the primary identifier shown to staff and owners.
/// </param>
/// <param name="Breed">
/// The dog's breed as provided by the owner or registration records.
/// </param>
/// <param name="DateOfBirth">
/// The dog's date of birth, formatted as an ISO‑8601 string (e.g., <c>2020-04-15</c>).
/// </param>
/// <param name="Sex">
/// The dog's biological sex (e.g., <c>Male</c>, <c>Female</c>).
/// </param>
public sealed record EditDogEndpointRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
