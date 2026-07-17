namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

public record GetDogSummaryEndpointResponse(Guid Id, string Name, string Breed);
