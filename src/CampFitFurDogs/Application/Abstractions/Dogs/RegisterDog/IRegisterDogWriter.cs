using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Abstractions.Dogs;

public interface IRegisterDogWriter
{
    Task WriteAsync(Dog dog, CancellationToken cancellationToken = default);
}
