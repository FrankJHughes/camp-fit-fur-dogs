namespace SharedKernel.Abstractions;

public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct);
    Task DispatchAsync(ICommand command, CancellationToken ct);
}
