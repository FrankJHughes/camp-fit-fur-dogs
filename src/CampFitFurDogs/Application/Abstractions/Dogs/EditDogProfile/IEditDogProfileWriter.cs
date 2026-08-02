using CampFitFurDogs.Domain.Dogs;
using Frank.Identity.Domain.Users;

namespace CampFitFurDogs.Application.Abstractions.Dogs;

public interface IEditDogProfileWriter
{
    Task WriteAsync(
        UserId ownerId,
        DogId id,
        DogName name,
        Breed breed,
        DateOnly dateOfBirth,
        Sex sex,
        CancellationToken cancellationToken);
}
