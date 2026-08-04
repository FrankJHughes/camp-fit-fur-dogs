namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

public sealed record EditDogEndpointRequest(
    string Name,
    string Breed,
    string DateOfBirth,
    string Sex);
