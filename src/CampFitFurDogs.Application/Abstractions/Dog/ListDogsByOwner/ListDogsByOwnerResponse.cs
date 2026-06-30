namespace CampFitFurDogs.Application.Abstractions.Dog.ListDogsByOwner;

public record DogSummary(Guid Id, string Name, string Breed);

public record ListDogsByOwnerResponse(IReadOnlyList<DogSummary> Dogs);
