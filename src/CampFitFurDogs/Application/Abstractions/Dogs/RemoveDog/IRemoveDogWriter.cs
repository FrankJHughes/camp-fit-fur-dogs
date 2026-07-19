namespace CampFitFurDogs.Application.Abstractions.Dogs;

public interface IRemoveDogWriter
{
    Task WriteAsync(Guid dogId, CancellationToken cancellationToken = default);
}
