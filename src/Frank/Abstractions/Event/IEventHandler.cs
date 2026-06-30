using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Abstractions.Event;

[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
