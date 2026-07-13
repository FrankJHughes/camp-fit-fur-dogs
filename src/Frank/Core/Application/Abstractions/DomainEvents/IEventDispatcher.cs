using Frank.Core.Domain;

namespace Frank.Core.Application.Abstractions.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
