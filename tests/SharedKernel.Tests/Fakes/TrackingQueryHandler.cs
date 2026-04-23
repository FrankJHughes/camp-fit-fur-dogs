using SharedKernel.Abstractions;

namespace SharedKernel.Tests.Fakes;

public sealed class TrackingQueryHandler<TQuery, TResponse>
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public int CallCount { get; private set; }
    public TQuery? LastQuery { get; private set; }
    public CancellationToken? LastToken { get; private set; }

    public Task<TResponse> Handle(TQuery query, CancellationToken ct)
    {
        CallCount++;
        LastQuery = query;
        LastToken = ct;

        if (typeof(TResponse) == typeof(string))
        {
            object result = $"handled: {query}";
            return Task.FromResult((TResponse)result);
        }

        return Task.FromResult(default(TResponse)!);
    }
}
