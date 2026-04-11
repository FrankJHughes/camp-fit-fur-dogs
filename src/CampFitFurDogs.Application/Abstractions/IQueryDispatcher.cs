namespace CampFitFurDogs.Application.Abstractions;

public interface IQueryDispatcher
{
    Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
