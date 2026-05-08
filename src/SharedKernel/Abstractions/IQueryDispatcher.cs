using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Abstractions;

[AutoRegister(ServiceLifetime.Scoped)]
public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
