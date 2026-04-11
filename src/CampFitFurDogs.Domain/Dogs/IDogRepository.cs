namespace CampFitFurDogs.Domain.Dogs;

public interface IDogRepository
{
    Task AddAsync(Dog dog, CancellationToken cancellationToken = default);
}
