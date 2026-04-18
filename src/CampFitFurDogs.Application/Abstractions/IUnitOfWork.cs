namespace CampFitFurDogs.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
