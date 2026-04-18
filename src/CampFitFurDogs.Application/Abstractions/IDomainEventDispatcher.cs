using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
