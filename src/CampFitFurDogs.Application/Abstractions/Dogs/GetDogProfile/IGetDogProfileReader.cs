namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

public interface IGetDogProfileReader
{
    Task<DogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct);
}
