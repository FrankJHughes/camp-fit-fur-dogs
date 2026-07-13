namespace Frank.Core.Application.Abstractions.Query;

public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct);
}
