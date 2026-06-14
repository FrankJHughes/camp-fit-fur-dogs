using Frank.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Events;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
