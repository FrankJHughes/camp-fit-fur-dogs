namespace CampFitFurDogs.Application.Abstractions;

public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
