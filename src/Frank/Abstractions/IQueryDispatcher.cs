using Microsoft.Extensions.DependencyInjection;
using Frank.DependencyInjection;

namespace Frank.Abstractions;

[AutoRegister(ServiceLifetime.Scoped)]
public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
