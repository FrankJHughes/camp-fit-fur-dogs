
namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogProfile;

public interface IGetDogProfileReader
{
    Task<GetDogProfileResponse?> ReadAsync(
        Guid dogId, Guid ownerId, CancellationToken ct);
}
