namespace CampFitFurDogs.Application.Abstractions;

public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct);
}
