namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

public sealed record EditDogProfileEndpointRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
