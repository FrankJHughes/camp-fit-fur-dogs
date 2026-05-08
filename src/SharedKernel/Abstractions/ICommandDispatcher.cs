using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Abstractions;

[AutoRegister(ServiceLifetime.Scoped)]
public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct);
    Task DispatchAsync(ICommand command, CancellationToken ct);
}
