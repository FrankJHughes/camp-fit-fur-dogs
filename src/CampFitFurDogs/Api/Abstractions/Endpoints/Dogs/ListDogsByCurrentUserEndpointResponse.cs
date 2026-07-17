namespace CampFitFurDogs.Api.Abstractions.Endpoints.Dogs;

public record ListDogsByCurrentUserEndpointResponse(IReadOnlyList<GetDogSummaryEndpointResponse> Dogs);
