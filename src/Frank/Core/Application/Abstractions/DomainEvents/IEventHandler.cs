using Frank.Core.Application.Registration;
using Frank.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Abstractions.DomainEvents;

[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
