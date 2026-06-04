using Microsoft.Extensions.DependencyInjection;
using Frank.DependencyInjection;

namespace Frank.Abstractions;

[AutoRegister(ServiceLifetime.Scoped)]
public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct);
    Task DispatchAsync(ICommand command, CancellationToken ct);
}
