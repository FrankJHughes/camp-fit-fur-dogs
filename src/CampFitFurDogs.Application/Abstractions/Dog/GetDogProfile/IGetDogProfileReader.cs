
namespace CampFitFurDogs.Application.Abstractions.Dog.GetDogProfile;

public interface IGetDogProfileReader
{
    Task<GetDogProfileResponse?> GetDogProfileAsync(
        Guid dogId, Guid ownerId, CancellationToken ct);
}
