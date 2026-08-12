namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

/// <summary>
/// Represents the payload required to register a new dog in the Camp Fit Fur Dogs
/// system.
/// <para>
/// This request is used by the <c>RegisterDogEndpoint</c> to capture the dog's
/// basic profile information during onboarding.
/// </para>
/// </summary>
/// <param name="Name">
/// The dog's display name as provided by the owner during registration.
/// </param>
/// <param name="Breed">
/// The dog's breed, used for classification, care requirements, and reporting.
/// </param>
/// <param name="DateOfBirth">
/// The dog's date of birth, formatted as an ISO‑8601 string (e.g., <c>2021-03-12</c>).
/// </param>
/// <param name="Sex">
/// The dog's biological sex (e.g., <c>Male</c>, <c>Female</c>).
/// </param>
public sealed record RegisterDogEndpointRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
