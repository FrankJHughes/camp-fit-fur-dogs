using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Events;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
