using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Application.DomainEvents;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    public DomainEventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IDomainEvent
    {
        // Materialize the handlers to a list
        var handlers = _provider.GetServices<IDomainEventHandler<TEvent>>().ToList();

        // No handlers? No-op.
        if (handlers.Count == 0)
            return;

        // Invoke handlers
        foreach (var handler in handlers)
        {
            await handler.Handle(domainEvent, ct);
        }
    }
}
