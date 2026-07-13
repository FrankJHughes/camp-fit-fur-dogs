using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.DomainEvents;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    public DomainEventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken ct = default)
        where TDomainEvent : IDomainEvent
    {
        // Materialize the handlers to a list
        var handlers = _provider.GetServices<IDomainEventHandler<TDomainEvent>>().ToList();

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

