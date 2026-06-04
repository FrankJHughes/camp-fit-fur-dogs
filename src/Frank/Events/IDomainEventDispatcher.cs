using Frank.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Events;

[AutoRegister(ServiceLifetime.Scoped, MaxRegistrationCount = 1)]
public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
