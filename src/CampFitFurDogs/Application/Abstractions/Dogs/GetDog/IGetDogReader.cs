
namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDog;

public interface IGetDogReader
{
    Task<GetDogResponse?> ReadAsync(
        Guid dogId, Guid ownerId, CancellationToken ct);
}
