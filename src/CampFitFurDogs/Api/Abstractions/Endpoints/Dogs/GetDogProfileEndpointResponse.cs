namespace CampFitFurDogs.Api.Endpoints.Dogs;

public record GetDogEndpointResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
