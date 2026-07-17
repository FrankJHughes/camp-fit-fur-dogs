namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

public sealed record RegisterDogEndpointRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
