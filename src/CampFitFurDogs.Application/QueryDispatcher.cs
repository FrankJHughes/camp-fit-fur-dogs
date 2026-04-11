using Microsoft.Extensions.DependencyInjection;

using CampFitFurDogs.Application.Abstractions;

namespace CampFitFurDogs.Application;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _provider;

    public QueryDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken ct)
    {
        // To Do: Avoid using reflection and dynamic in production code.
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResponse));

        var handler = _provider.GetRequiredService(handlerType);

        return ((dynamic)handler).Handle((dynamic)query, ct);
    }
}
