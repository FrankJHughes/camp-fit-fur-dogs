using Frank.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions;

[AutoRegister(ServiceLifetime.Scoped)]
public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
