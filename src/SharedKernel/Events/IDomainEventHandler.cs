using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Events;

[AutoRegister(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
