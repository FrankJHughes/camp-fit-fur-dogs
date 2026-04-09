namespace CampFitFurDogs.Application.Abstractions;

public interface ICommandDispatcher
{
    Task<TResponse> Dispatch<TResponse>(ICommand<TResponse> command, CancellationToken ct);
}
