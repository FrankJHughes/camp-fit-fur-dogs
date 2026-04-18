using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Application.Abstractions;

public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken = default);
}
