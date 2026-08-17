using Frank.Core.Application.Registration;
using Frank.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.DomainEvents;

/// <summary>
/// Defines the contract for handling a domain event within a domain‑driven
/// design (DDD) event pipeline.
///
/// <para>
/// A domain event represents something meaningful that has occurred inside the
/// domain model. Implementations of <see cref="IDomainEventHandler{TEvent}"/>
/// react to these events by performing side‑effects, triggering workflows, or
/// updating read models.
/// </para>
///
/// <para>
/// The <see cref="RegistrationAttribute"/> ensures that each handler is
/// automatically registered into the dependency injection container with a
/// scoped lifetime and concrete type registration, allowing the dispatcher to
/// resolve all handlers for a given event type.
/// </para>
/// </summary>
/// <typeparam name="TEvent">
/// The type of domain event being handled. Must implement
/// <see cref="IDomainEvent"/>.
/// </typeparam>
[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Processes the specified domain event asynchronously.
    ///
    /// <para>
    /// Multiple handlers may exist for the same event type. Each handler is
    /// invoked independently by the dispatcher. The returned task completes
    /// when the handler has finished processing the event or when cancellation
    /// is requested via the provided token.
    /// </para>
    /// </summary>
    /// <param name="domainEvent">
    /// The domain event to handle.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
