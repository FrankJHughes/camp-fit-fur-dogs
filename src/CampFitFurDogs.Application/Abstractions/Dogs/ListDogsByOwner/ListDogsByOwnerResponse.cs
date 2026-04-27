namespace CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;

public record DogSummary(Guid Id, string Name, string Breed);

public record ListDogsByOwnerResponse(IReadOnlyList<DogSummary> Dogs);
