using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Events;

[AutoRegister(ServiceLifetime.Scoped, MaxRegistrationCount = 1)]
public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
