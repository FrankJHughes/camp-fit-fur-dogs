namespace Frank.Core.Application.Abstractions.Cqrs.Queries;

public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
