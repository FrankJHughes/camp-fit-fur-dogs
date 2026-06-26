using Frank.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Event;

public sealed class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _provider;

    public EventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IEvent
    {
        // Materialize the handlers to a list
        var handlers = _provider.GetServices<IEventHandler<TEvent>>().ToList();

        // No handlers? No-op.
        if (handlers.Count == 0)
            return;

        // Invoke handlers
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, ct);
        }
    }
}

