namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

public interface IGetDogProfileReader
{
    Task<GetDogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct);
}
