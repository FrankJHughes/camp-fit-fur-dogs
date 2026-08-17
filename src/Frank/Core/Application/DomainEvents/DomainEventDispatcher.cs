using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.DomainEvents;

/// <summary>
/// Dispatches domain events to all registered handlers for the event type.
///
/// <para>
/// A <see cref="DomainEventDispatcher"/> is responsible for invoking every
/// <see cref="IDomainEventHandler{TDomainEvent}"/> associated with a given
/// domain event. Unlike CQRS commands or queries, domain events represent
/// *post‑state‑change notifications* emitted by aggregates or domain services.
/// </para>
///
/// <para>
/// The dispatcher resolves all handlers from dependency injection, materializes
/// them, and invokes them sequentially. If no handlers are registered for the
/// event type, the dispatch is a no‑op.
/// </para>
/// </summary>
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
    /// </summary>
    /// <param name="provider">
    /// The service provider used to resolve domain event handlers.
    /// </param>
    public DomainEventDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Dispatches a domain event to all registered handlers for the event type.
    /// </summary>
    /// <typeparam name="TDomainEvent">
    /// The type of domain event being dispatched.
    /// </typeparam>
    /// <param name="domainEvent">
    /// The domain event instance to dispatch.
    /// </param>
    /// <param name="ct">
    /// A cancellation token for the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous dispatch operation.
    /// </returns>
    /// <remarks>
    /// Handlers are invoked sequentially. If no handlers are registered, the
    /// dispatch completes immediately without side effects.
    /// </remarks>
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
