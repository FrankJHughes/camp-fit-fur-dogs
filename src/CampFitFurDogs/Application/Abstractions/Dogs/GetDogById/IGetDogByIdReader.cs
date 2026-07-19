
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;

public interface IGetDogByIdReader
{
    Task<Dog?> ReadAsync(Guid dogId, CancellationToken ct);
}
