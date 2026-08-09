using Frank.Core.Domain;

namespace Frank.Core.Application.Abstractions.DomainEvents;

/// <summary>
/// Defines the contract for dispatching domain events to their corresponding
/// handlers within a domain‑driven design (DDD) event pipeline.
///
/// <para>
/// A domain event represents something significant that has occurred within the
/// domain model. The <see cref="IDomainEventDispatcher"/> is responsible for
/// locating and invoking all handlers associated with the event type, ensuring
/// that side‑effects and reactions are executed consistently.
/// </para>
///
/// <para>
/// Dispatching is asynchronous and may involve multiple handlers. The dispatcher
/// coordinates their execution and respects cancellation via the provided token.
/// </para>
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches the specified domain event by invoking all registered handlers
    /// associated with its type.
    ///
    /// <para>
    /// Each handler processes the event independently. The returned task
    /// completes when all handlers have finished or when cancellation is
    /// requested.
    /// </para>
    /// </summary>
    /// <typeparam name="TEvent">
    /// The type of domain event being dispatched. Must implement
    /// <see cref="IDomainEvent"/>.
    /// </typeparam>
    /// <param name="domainEvent">
    /// The domain event to dispatch.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that may be used to cancel the dispatch operation.
    /// </param>
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
